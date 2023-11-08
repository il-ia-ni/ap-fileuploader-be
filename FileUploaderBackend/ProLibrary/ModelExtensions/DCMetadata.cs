using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProLibrary.Models
{
    public partial class DcMetadata
    {
        [Timestamp]
        public byte[]? RowVersion { get; set; }  // this prop is used for tracking pessimistic concurrency

        public override string ToString()
        {
            return "Item " + this.ItemName + " is of type " + this.ItemType;
        }
    }
}
