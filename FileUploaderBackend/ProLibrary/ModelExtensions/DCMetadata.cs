using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProLibrary.Models
{
    public partial class DCMetadata
    {
        [Timestamp]
        public byte[]? RowVersion { get; set; }  // this prop is used for tracking lazy concurrency

        public override string ToString()
        {
            return "This is a test value of DCMetadata class";
        }
    }
}
