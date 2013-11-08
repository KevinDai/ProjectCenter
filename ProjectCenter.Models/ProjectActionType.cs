using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public enum ProjectActionType
    {
        Update = 0,
        Delete = 1,
        AddComment = 2,
        DeleteComment = 3,
        AddAttachment = 4,
        DeleteAttachment = 5,
        ChangeStatus = 6,
        Create = 7
    }
}
