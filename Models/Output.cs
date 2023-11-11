using System;
using System.Collections.Generic;

namespace ProjectPRN221.Models
{
    public partial class Output
    {
        public Output()
        {
            OutputInfos = new HashSet<OutputInfo>();
        }

        public string Id { get; set; } = null!;
        public DateTime? DateOutput { get; set; }

        public virtual ICollection<OutputInfo> OutputInfos { get; set; }
    }
}
