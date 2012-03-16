using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace OmidID.Web.Security {
    public class EFMembershipProvider<TUser, TKey> : System.Web.Security.MembershipProvider
        where TUser : class
        where TKey : struct {

        #region Const

        private const int PASSWORD_SIZE = 14;
        private const int SALT_SIZE_IN_BYTES = 16;

        #endregion

        #region Variables

        private bool _EnablePasswordReset;
        private bool _EnablePasswordRetrieval;
        private int _MaxInvalidPasswordAttempts;
        private int _MinRequiredNonAlphanumericCharacters;
        private int _MinRequiredPasswordLength;
        private int _PasswordAttemptWindow;
        private MembershipPasswordFormat _PasswordFormat;
        private string _PasswordStrengthRegularExpression;
        private bool _RequiresQuestionAndAnswer;
        private bool _RequiresUniqueEmail;
        private string _connectionString;
        private MembershipPasswordCompatibilityMode _LegacyPasswordCompatibilityMode;
        private string s_HashAlgorithm;

        #endregion

        #region Properties

        public override string ApplicationName { get; set; }
        public override bool EnablePasswordReset { get { return _EnablePasswordReset; } }
        public override bool EnablePasswordRetrieval { get { return _EnablePasswordRetrieval; } }
        public override int MaxInvalidPasswordAttempts { get { return _MaxInvalidPasswordAttempts; } }
        public override int MinRequiredNonAlphanumericCharacters { get { return _MinRequiredNonAlphanumericCharacters; } }
        public override int MinRequiredPasswordLength { get { return _MinRequiredPasswordLength; } }
        public override int PasswordAttemptWindow { get { return _PasswordAttemptWindow; } }
        public override System.Web.Security.MembershipPasswordFormat PasswordFormat { get { return _PasswordFormat; } }
        public override string PasswordStrengthRegularExpression { get { return _PasswordStrengthRegularExpression; } }
        public override bool RequiresQuestionAndAnswer { get { return _RequiresQuestionAndAnswer; } }
        public override bool RequiresUniqueEmail { get { return _RequiresUniqueEmail; } }

        public string ConnectionString { get { return _connectionString; } }
        public string TablePrefix { get; private set; }
        public string TableSchema { get; private set; }

        public DataContext.IUserContext<TUser, TKey> Context { get; private set; }
        public Mapper.IUserMapper<TUser> Mapper { get; private set; }
        internal Mapper.ClassHelper<TUser, Mapper.UserColumnType, UserColumnAttribute> Helper { get; set; }

        public bool SupportApplication { get; set; }
        public bool SupportEmail { get; private set; }
        public bool SupportComment { get; private set; }
        public bool SupportIsApproved { get; private set; }
        public bool SupportIsAnonymous { get; private set; }

        public bool SupportQuestionAndPassword { get; private set; }
        public bool SupportLockUser { get; private set; }
        public bool SupportAutoLockUser { get; private set; }
        public bool SupportAutoLockUserQuestion { get; private set; }

        public bool SupportCreateOn { get; private set; }
        public bool SupportLastLoginDate { get; private set; }
        public bool SupportLastActivityDate { get; private set; }
        public bool SupportLastLockoutDate { get; private set; }
        public bool SupportLastPasswordChangedDate { get; private set; }

        #endregion

        #region Initialize

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
            if (config == null)
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(name))
                name = "EFMembership";
            if (config["description"] == null) {
                config.Remove("description");
                config.Add("description", "Ado.net Entity Framework Membership Provider");
            }
            base.Initialize(name, config);

            TablePrefix = config.GetStringValue("tablePrefix", "");
            TableSchema = config.GetStringValue("tableSchema", "");
            _EnablePasswordRetrieval = config.GetBooleanValue("enablePasswordRetrieval", false);
            _EnablePasswordReset = config.GetBooleanValue("enablePasswordReset", true);
            _RequiresQuestionAndAnswer = config.GetBooleanValue("requiresQuestionAndAnswer", true);
            _RequiresUniqueEmail = config.GetBooleanValue("requiresUniqueEmail", true);
            _MaxInvalidPasswordAttempts = config.GetIntValue("maxInvalidPasswordAttempts", 5, false, 0);
            _PasswordAttemptWindow = config.GetIntValue("passwordAttemptWindow", 10, false, 0);
            _MinRequiredPasswordLength = config.GetIntValue("minRequiredPasswordLength", 7, false, 128);
            _MinRequiredNonAlphanumericCharacters = config.GetIntValue("minRequiredNonalphanumericCharacters", 1, true, 128);

            _PasswordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
            if (_PasswordStrengthRegularExpression != null) {
                _PasswordStrengthRegularExpression = _PasswordStrengthRegularExpression.Trim();
                if (_PasswordStrengthRegularExpression.Length != 0) {
                    try {
                        Regex regex = new Regex(_PasswordStrengthRegularExpression);
                    } catch (ArgumentException e) {
                        throw new ProviderException(e.Message, e);
                    }
                }
            } else {
                _PasswordStrengthRegularExpression = string.Empty;
            }
            if (_MinRequiredNonAlphanumericCharacters > _MinRequiredPasswordLength)
                throw new ProviderException("MinRequiredNonalphanumericCharacters_can_not_be_more_than_MinRequiredPasswordLength".Resource());

            string temp = config["connectionStringName"];
            if (temp == null || temp.Length < 1)
                throw new ProviderException("Connection_name_not_specified".Resource());
            if (System.Configuration.ConfigurationManager.ConnectionStrings[temp] == null)
                throw new ProviderException("Connection_not_found".Resource());
            _connectionString = temp;

            ApplicationName = config.GetStringValue("applicationName", "/");
            if (ApplicationName.Length > 256)
                throw new ProviderException("Provider_application_name_too_long".Resource());

            string strTemp = config.GetStringValue("passwordFormat", "Hashed");
            switch (strTemp) {
                case "Clear": _PasswordFormat = MembershipPasswordFormat.Clear; break;
                case "Encrypted": _PasswordFormat = MembershipPasswordFormat.Encrypted; break;
                case "Hashed": _PasswordFormat = MembershipPasswordFormat.Hashed; break;
                default:
                    throw new ProviderException("Provider_bad_password_format".Resource());
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed && EnablePasswordRetrieval)
                throw new ProviderException("Provider_can_not_retrieve_hashed_password".Resource());

            string passwordCompactMode = config["passwordCompatMode"];
            if (!string.IsNullOrEmpty(passwordCompactMode)) {
                this._LegacyPasswordCompatibilityMode = (MembershipPasswordCompatibilityMode)Enum.Parse(typeof(MembershipPasswordCompatibilityMode), passwordCompactMode);
            }

            var type = typeof(TUser);
            var attr = type.GetCustomAttributes(typeof(EFDataMapperAttribute), false);

            Helper = new Mapper.ClassHelper<TUser, Mapper.UserColumnType, UserColumnAttribute>(TablePrefix, TableSchema);
            if (attr != null && attr.Length > 0) {
                var mapperAttr = attr[0] as EFDataMapperAttribute;
                Mapper = Activator.CreateInstance(mapperAttr.MapperType) as Mapper.IUserMapper<TUser>;

                if (Mapper == null)
                    throw new ProviderException("Reflection_can_not_cast_object".Resource(mapperAttr.MapperType.FullName,
                                                                                          typeof(Mapper.IUserMapper<TUser>).FullName));
            } else {
                Mapper = new Mapper.UserAutoMapper<TUser>(Helper);
            }

            if (!Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.Username))
                throw new ProviderException("Reflection_property_is_required".Resource("Username"));
            if (!Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.UserID))
                throw new ProviderException("Reflection_property_is_required".Resource("UserID"));
            if (!Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.Password))
                throw new ProviderException("Reflection_property_is_required".Resource("Password"));
            if (!Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.PasswordFormat))
                throw new ProviderException("Reflection_property_is_required".Resource("PasswordFormat"));
            if (!Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.PasswordSalt))
                throw new ProviderException("Reflection_property_is_required".Resource("PasswordSalt"));

            SupportApplication = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.Application);
            SupportEmail = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.Email);
            SupportComment = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.Comment);
            SupportIsApproved = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.IsApproved);
            SupportIsAnonymous = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.IsAnonymous);

            SupportQuestionAndPassword = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.PasswordQuestion) &&
                                         Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.PasswordAnswer);
            SupportLockUser = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.IsLockedOut);
            SupportAutoLockUser = SupportLockUser && Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.FailedPasswordAttemptCount) &&
                                                     Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.FailedPasswordAttemptWindowStart);
            SupportAutoLockUserQuestion = SupportLockUser && Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.FailedPasswordAnswerAttemptCount) &&
                                                             Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.FailedPasswordAnswerAttemptWindowStart);

            SupportCreateOn = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.CreateOn);
            SupportLastLoginDate = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.LastLoginDate);
            SupportLastActivityDate = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.LastActivityDate);
            SupportLastLockoutDate = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.LastLockoutDate);
            SupportLastPasswordChangedDate = Helper.Properties.ContainsKey(Security.Mapper.UserColumnType.LastPasswordChangedDate);


            attr = type.GetCustomAttributes(typeof(EFDataContextAttribute), false);
            if (attr != null && attr.Length > 0) {
                var contextAttr = attr[0] as EFDataContextAttribute;
                Context = Activator.CreateInstance(contextAttr.ContextType) as DataContext.IUserContext<TUser, TKey>;
                if (Context == null) {
                    throw new ProviderException("Reflection_can_not_cast_object".Resource(contextAttr.ContextType.FullName,
                                                                                          typeof(DataContext.IUserContext<TUser, TKey>).FullName));
                }
            } else {
                Context = new DataContext.DefaultUserContext<TUser, TKey>(Helper);
            }
            Context.Initialize(this);

        }

        #endregion

        #region Validators

        protected virtual bool IsValidUsername(string Username) {
            if (string.IsNullOrWhiteSpace(Username)) return false;
            return true;
        }

        protected virtual bool IsValidEmail(string Email) {
            if (string.IsNullOrWhiteSpace(Email)) return false;

            var charNum1 = Email.IndexOf('@');
            if (charNum1 < 2) return false;

            var charNum2 = Email.IndexOf('.', charNum1 + 1);
            if (charNum2 < 1 || charNum2 + 1 > Email.Length) return false;

            return true;
        }

        #endregion

        #region Password And Encoding

        internal string EncodeAnswerPassword(string pass, MembershipPasswordFormat passwordFormat, string salt) {
            return EncodePassword(pass.ToLower().Trim(), passwordFormat, salt);
        }

        internal string EncodePassword(string pass, MembershipPasswordFormat passwordFormat, string salt) {
            if (passwordFormat == MembershipPasswordFormat.Clear) {
                return pass;
            }
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Convert.FromBase64String(salt);
            byte[] inArray = null;
            if (passwordFormat == MembershipPasswordFormat.Hashed) {
                HashAlgorithm hashAlgorithm = this.GetHashAlgorithm();
                if (hashAlgorithm is KeyedHashAlgorithm) {
                    KeyedHashAlgorithm algorithm2 = (KeyedHashAlgorithm)hashAlgorithm;
                    if (algorithm2.Key.Length == src.Length) {
                        algorithm2.Key = src;
                    } else if (algorithm2.Key.Length < src.Length) {
                        byte[] dst = new byte[algorithm2.Key.Length];
                        Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
                        algorithm2.Key = dst;
                    } else {
                        int num2;
                        byte[] buffer5 = new byte[algorithm2.Key.Length];
                        for (int i = 0; i < buffer5.Length; i += num2) {
                            num2 = Math.Min(src.Length, buffer5.Length - i);
                            Buffer.BlockCopy(src, 0, buffer5, i, num2);
                        }
                        algorithm2.Key = buffer5;
                    }
                    inArray = algorithm2.ComputeHash(bytes);
                } else {
                    byte[] buffer6 = new byte[src.Length + bytes.Length];
                    Buffer.BlockCopy(src, 0, buffer6, 0, src.Length);
                    Buffer.BlockCopy(bytes, 0, buffer6, src.Length, bytes.Length);
                    inArray = hashAlgorithm.ComputeHash(buffer6);
                }
            } else {
                byte[] buffer7 = new byte[src.Length + bytes.Length];
                Buffer.BlockCopy(src, 0, buffer7, 0, src.Length);
                Buffer.BlockCopy(bytes, 0, buffer7, src.Length, bytes.Length);
                inArray = this.EncryptPassword(buffer7, _LegacyPasswordCompatibilityMode);
            }
            return Convert.ToBase64String(inArray);
        }

        private HashAlgorithm GetHashAlgorithm() {
            if (this.s_HashAlgorithm != null) {
                return HashAlgorithm.Create(this.s_HashAlgorithm);
            }
            string hashAlgorithmType = Membership.HashAlgorithmType;
            if (((this._LegacyPasswordCompatibilityMode == MembershipPasswordCompatibilityMode.Framework20)) && (hashAlgorithmType != "MD5")) {
                hashAlgorithmType = "SHA1";
            }
            HashAlgorithm algorithm = HashAlgorithm.Create(hashAlgorithmType);
            if (algorithm == null) {
                throw new ConfigurationErrorsException("Invalid_hash_algorithm_type".Resource(hashAlgorithmType));
            }
            this.s_HashAlgorithm = hashAlgorithmType;
            return algorithm;
        }

        internal string UnEncodePassword(string pass, MembershipPasswordFormat passwordFormat) {
            switch (passwordFormat) {
                case MembershipPasswordFormat.Clear:
                    return pass;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Provider_can_not_decode_hashed_password".Resource());
                default:
                    byte[] bIn = Convert.FromBase64String(pass);
                    byte[] bRet = DecryptPassword(bIn);
                    if (bRet == null)
                        return null;
                    return Encoding.Unicode.GetString(bRet, SALT_SIZE_IN_BYTES, bRet.Length - SALT_SIZE_IN_BYTES);
            }
        }

        internal string GenerateSalt() {
            byte[] buf = new byte[SALT_SIZE_IN_BYTES];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public virtual string GeneratePassword() {
            return Membership.GeneratePassword(
                      MinRequiredPasswordLength < PASSWORD_SIZE ? PASSWORD_SIZE : MinRequiredPasswordLength,
                      MinRequiredNonAlphanumericCharacters);
        }

        internal bool CheckPassword(TUser User, string Password, MembershipPasswordFormat Format, string Salt, bool UpdateActivity, bool UpdateLogin, bool LockIsPossible) {
            if (Mapper.IsLockedOut(User)) return false;

            var Hashed = EncodePassword(Password, Format, Salt);
            var storedPassword = Mapper.Password(User);
            if (Hashed.Equals(storedPassword, StringComparison.OrdinalIgnoreCase)) {
                if (UpdateActivity && SupportLastActivityDate)
                    Mapper.LastActivityDate(User, DateTime.UtcNow);

                if (UpdateLogin && SupportLastLoginDate)
                    Mapper.LastLoginDate(User, DateTime.UtcNow);

                if (SupportAutoLockUser && LockIsPossible) {
                    Mapper.FailedPasswordAttemptCount(User, 0);
                    Mapper.FailedPasswordAttemptWindowStart(User, new DateTime(1800, 1, 1));
                }

                return true;
            } else {
                if (LockIsPossible && SupportAutoLockUser) {
                    var faileds = Mapper.FailedPasswordAttemptCount(User) + 1;
                    var failedOn = Mapper.FailedPasswordAttemptWindowStart(User);

                    var utc = DateTime.UtcNow;
                    if (failedOn.AddMinutes(this.PasswordAttemptWindow) <= utc)
                        Mapper.FailedPasswordAttemptCount(User, 1);
                    else
                        Mapper.FailedPasswordAttemptCount(User, faileds);

                    Mapper.FailedPasswordAttemptWindowStart(User, utc);
                    if (faileds >= this.MaxInvalidPasswordAttempts) {
                        Mapper.IsLockedOut(User, true);
                    }
                }

                return false;
            }
        }


        internal bool CheckAnswerPassword(TUser User, string Answer, MembershipPasswordFormat Format, string Salt, bool LockIsPossible) {
            if (Mapper.IsLockedOut(User)) return false;

            var Hashed = EncodePassword(Answer.ToLower(), Format, Salt);
            if (Hashed.Equals(Mapper.Get<string>(User, Security.Mapper.UserColumnType.Password), StringComparison.OrdinalIgnoreCase)) {
                if (SupportAutoLockUser && LockIsPossible) {
                    Mapper.FailedPasswordAnswerAttemptCount(User, 0);
                    Mapper.FailedPasswordAnswerAttemptWindowStart(User, new DateTime(1800, 1, 1));
                }

                return true;
            } else {
                if (LockIsPossible && SupportAutoLockUser) {
                    var faileds = Mapper.FailedPasswordAnswerAttemptCount(User) + 1;
                    var failedOn = Mapper.FailedPasswordAnswerAttemptWindowStart(User);

                    var utc = DateTime.UtcNow;
                    if (failedOn.AddMinutes(this.PasswordAttemptWindow) <= utc)
                        Mapper.FailedPasswordAnswerAttemptCount(User, 1);
                    else
                        Mapper.FailedPasswordAnswerAttemptCount(User, faileds);

                    Mapper.FailedPasswordAnswerAttemptWindowStart(User, utc);
                    if (this.MaxInvalidPasswordAttempts >= faileds) {
                        Mapper.IsLockedOut(User, true);
                    }
                }

                return false;
            }
        }

        #endregion

        #region Password And Security

        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            var user = Context.GetUser(username);
            if (user == null) return false;

            var passwordFormat = Mapper.PasswordFormat(user);
            var passwordSalt = Mapper.PasswordSalt(user);

            var check = CheckPassword(user, oldPassword, passwordFormat, passwordSalt, false, false, true);

            if (!check) {
                Context.Update(user);
                return false;
            }

            if (PasswordFormat != passwordFormat) {
                passwordFormat = PasswordFormat;
                passwordSalt = GenerateSalt();
            }

            Mapper.Password(user, EncodePassword(newPassword, passwordFormat, passwordSalt));
            Mapper.LastPasswordChangedDate(user, DateTime.UtcNow);
            Mapper.FailedPasswordAnswerAttemptCount(user, 0);
            Mapper.FailedPasswordAttemptCount(user, 0);

            Context.Update(user);
            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            var user = Context.GetUser(username);
            if (user == null) return false;

            var passwordFormat = Mapper.PasswordFormat(user);
            var passwordSalt = Mapper.PasswordSalt(user);

            var check = CheckPassword(user, password, passwordFormat, passwordSalt, false, false, true);

            if (!check) {
                Context.Update(user);
                return false;
            }

            Mapper.PasswordQuestion(user, newPasswordQuestion);
            Mapper.PasswordAnswer(user, EncodePassword(newPasswordAnswer.ToLower(), passwordFormat, passwordSalt));

            Context.Update(user);
            return true;
        }

        public override string GetPassword(string username, string answer) {
            if (!EnablePasswordRetrieval)
                throw new NotSupportedException("Membership_PasswordRetrieval_not_supported".Resource());

            if (!SupportQuestionAndPassword)
                throw new NotSupportedException("QuestionAndAnswer_not_supported".Resource());

            var user = Context.GetUser(username);
            var passwordFormat = Mapper.PasswordFormat(user);
            var passwordSalt = Mapper.PasswordSalt(user);
            var dbAnswer = Mapper.PasswordAnswer(user);

            if (!CheckAnswerPassword(user, answer, passwordFormat, passwordSalt, true)) {
                Context.Update(user);
                return null;
            }

            Mapper.FailedPasswordAnswerAttemptCount(user, 0);
            Mapper.FailedPasswordAttemptCount(user, 0);
            Context.Update(user);

            return UnEncodePassword(Mapper.Password(user), passwordFormat);
        }

        public override string ResetPassword(string username, string answer) {
            if (!EnablePasswordReset)
                throw new NotSupportedException("Not_configured_to_support_password_resets".Resource());

            if (!SupportQuestionAndPassword)
                throw new NotSupportedException("QuestionAndAnswer_not_supported".Resource());

            var user = Context.GetUser(username);
            var passwordFormat = Mapper.PasswordFormat(user);
            var passwordSalt = Mapper.PasswordSalt(user);

            if (answer != null) {
                var dbAnswer = Mapper.PasswordAnswer(user);

                if (!CheckAnswerPassword(user, answer, passwordFormat, passwordSalt, true)) {
                    Context.Update(user);
                    return null;
                }
            }

            var newPassword = GeneratePassword();

            if (PasswordFormat != passwordFormat) {
                passwordFormat = PasswordFormat;
                passwordSalt = GenerateSalt();
            }

            Mapper.Password(user, EncodePassword(newPassword, passwordFormat, passwordSalt));
            Mapper.LastPasswordChangedDate(user, DateTime.UtcNow);
            Mapper.FailedPasswordAnswerAttemptCount(user, 0);
            Mapper.FailedPasswordAttemptCount(user, 0);

            Context.Update(user);
            return newPassword;
        }

        public override bool ValidateUser(string username, string password) {
            var user = Context.GetUser(username);
            if (user == null) return false;

            var passwordFormat = Mapper.Get<MembershipPasswordFormat>(user, Security.Mapper.UserColumnType.PasswordFormat);
            var passwordSalt = Mapper.Get<string>(user, Security.Mapper.UserColumnType.PasswordSalt);

            var check = CheckPassword(user, password, passwordFormat, passwordSalt, true, true, true);
            Context.Update(user);

            return check;
        }

        #endregion

        #region User Status

        public override bool UnlockUser(string userName) {
            var user = Context.GetUser(userName);
            if (user == null) return false;

            Mapper.IsLockedOut(user, false);
            Mapper.LastLockoutDate(user, DateTime.UtcNow);
            Context.Update(user);

            return true;
        }

        #endregion

        #region User List

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            var collection = new MembershipUserCollection();
            foreach (var item in Context.FindByUsername(usernameToMatch, pageSize, pageIndex, out totalRecords))
                collection.Add(Mapper.To(this.Name, item));

            return collection;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            var collection = new MembershipUserCollection();
            foreach (var item in Context.FindByUsername(emailToMatch, pageSize, pageIndex, out totalRecords))
                collection.Add(Mapper.To(this.Name, item));

            return collection;
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            var collection = new MembershipUserCollection();
            foreach (var item in Context.UserList(pageSize, pageIndex, out totalRecords))
                collection.Add(Mapper.To(this.Name, item));

            return collection;
        }

        public override int GetNumberOfUsersOnline() {
            if (!SupportLastActivityDate)
                throw new NotSupportedException("GetNumberOfUsersOnline_not_supported");

            return Context.NumberOfOnlineUsers(Membership.UserIsOnlineTimeWindow);
        }

        #endregion

        #region Get User

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline) {
            var user = Context.GetUser(username);
            if (user == null) return null;

            if (userIsOnline) {
                Mapper.LastActivityDate(user, DateTime.UtcNow);
                Context.Update(user);
            }

            return Mapper.To(this.Name, user);
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            var user = Context.GetUser((TKey)providerUserKey);
            if (user == null) return null;

            if (userIsOnline) {
                Mapper.LastActivityDate(user, DateTime.UtcNow);
                Context.Update(user);
            }

            return Mapper.To(this.Name, user);
        }

        public override string GetUserNameByEmail(string email) {
            var user = Context.GetUserByEmail(email);
            if (user == null) return null;

            return Mapper.Email(user);
        }

        #endregion

        #region Create User

        public override System.Web.Security.MembershipUser CreateUser(string username, string password,
                                                                      string email, string passwordQuestion, string passwordAnswer,
                                                                      bool isApproved, object providerUserKey,
                                                                      out System.Web.Security.MembershipCreateStatus status) {
            if (!IsValidUsername(username)) {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            if (providerUserKey != null && (providerUserKey is TKey)) {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            if (!IsValidEmail(email)) {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }


            int count = 0;
            for (int i = 0; i < password.Length; i++)
                if (!char.IsLetterOrDigit(password, i))
                    count++;

            if (count < MinRequiredNonAlphanumericCharacters) {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (PasswordStrengthRegularExpression.Length > 0) {
                if (!Regex.IsMatch(password, PasswordStrengthRegularExpression)) {
                    status = MembershipCreateStatus.InvalidPassword;
                    return null;
                }
            }


            var passArgs = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(passArgs);

            if (passArgs.Cancel) {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (Context.GetUser(username) != null) {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (providerUserKey != null)
                if (Context.GetUser((TKey)providerUserKey) != null) {
                    status = MembershipCreateStatus.DuplicateProviderUserKey;
                    return null;
                }

            if (Context.GetUserByEmail(email) != null) {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var user = Helper.New();

            var passwordSalt = GenerateSalt();
            var encodedPassword = EncodePassword(password, PasswordFormat, passwordSalt);

            if (passwordAnswer != null) {
                var encodedAnswer = EncodeAnswerPassword(passwordAnswer, PasswordFormat, passwordSalt);
                Mapper.PasswordQuestion(user, passwordQuestion);
                Mapper.PasswordAnswer(user, encodedAnswer);
            }

            if (providerUserKey != null)
                Mapper.UserID(user, providerUserKey);

            Mapper.Username(user, username);
            Mapper.Password(user, encodedPassword);
            Mapper.PasswordFormat(user, PasswordFormat);
            Mapper.PasswordSalt(user, passwordSalt);
            Mapper.Email(user, email);
            Mapper.IsApproved(user, isApproved);

            var now = DateTime.UtcNow;
            Mapper.CreateOn(user, now);
            Mapper.LastActivityDate(user, now);
            Mapper.LastLockoutDate(user, now);
            Mapper.LastLoginDate(user, now);
            Mapper.LastPasswordChangedDate(user, now);
            Mapper.FailedPasswordAttemptWindowStart(user, now);
            Mapper.FailedPasswordAnswerAttemptWindowStart(user, now);

            Mapper.IsLockedOut(user, false);
            Mapper.IsAnonymous(user, false);
            Mapper.Comment(user, "");

            Context.Add(user);

            status = MembershipCreateStatus.Success;
            return Mapper.To(this.Name, user);
        }

        #endregion

        #region Update and Delete User

        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            var user = Context.GetUser(username);
            if (user == null) return false;

            try {
                Context.Delete(user);
                return true;
            } catch {
                return false;
            }
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user) {
            var entity = Context.GetUser(user.UserName);
            Context.Update(Mapper.To(entity, user));
        }

        #endregion

        #region Extended Function For Role And Profile Provider

        public TKey GetUserID(string username) {
            return Context.GetUserID(username);
        }

        public TKey[] GetUserID(string[] username) {
            return Context.GetUserID(username);
        }

        public string GetUsername(TKey UserID) {
            return Context.GetUsername(UserID);
        }

        public string[] GetUsername(object[] UserID) {
            return Context.GetUsername(UserID.Cast<TKey>().ToArray());
        }

        #endregion

    }
}
