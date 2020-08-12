using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreWebApiHomework.Models
{
    interface ITrack
    {
        DateTime? DateModified { get; set; }
        bool IsDeleted { get; set; }
    }
}
