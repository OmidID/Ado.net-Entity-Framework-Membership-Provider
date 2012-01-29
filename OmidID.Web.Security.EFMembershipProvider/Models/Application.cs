using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace OmidID.Web.Security.Models {
    public abstract class Application {

        public abstract string Name { get; set; }

    }

}
