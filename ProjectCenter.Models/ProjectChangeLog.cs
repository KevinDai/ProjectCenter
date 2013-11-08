using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class ProjectChangeLog
    {
        public string Id
        {
            get;
            set;
        }

        public string ProjectId
        {
            get;
            set;
        }

        public string UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public int ActionType
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public string ActionTypeString
        {
            get
            {
                var actionType = (ProjectActionType)ActionType;
                switch (actionType)
                {
                    case ProjectActionType.Create:
                        return "创建项目";
                    case ProjectActionType.Update:
                        return "编辑";
                    case ProjectActionType.AddAttachment:
                        return "增加附件";
                    case ProjectActionType.DeleteAttachment:
                        return "删除附件";
                    case ProjectActionType.AddComment:
                        return "增加评论";
                    case ProjectActionType.DeleteComment:
                        return "删除评论";
                    case ProjectActionType.ChangeStatus:
                        return "更改项目状态";
                    default:
                        return "未知类型";
                }
            }
        }
    }
}
