
function computedFormatPrice(model, field) {
    return ko.computed({
        read: function () {
            return model[field]()
                ? thousandthFormatted(model[field]().toFixed(2))
                : (0).toFixed(2);
        },
        write: function (value) {
            value = parseFloat(value.replace(/[^\.\d]/g, ""));
            model[field](isNaN(value) ? 0 : value); // Write to underlying storage
        },
        owner: model
    });
}

function thousandthFormatted(str) {
    var re = /\d{1,3}(?=(\d{3})+$)/g;
    return str.replace(/^(\d+)((\.\d+)?)$/, function (s, s1, s2) { return s1.replace(re, "$&,") + s2; });
}

var projectEditViewModel = function (project) {
    var self = this,
        formattedDateStr = function (datetime) {
            return datetime ? datetime.replace(/\//g, '-').split(" ")[0] : datetime;
        },
        makeCheckArray = function (array) {
            if (array) {
                for (var i = 0; i < array.length; i++) {
                    array[i].Checked = ko.observable(false);
                }
            }
            return array;
        };
    self.EnableEditProject = ko.observable(false);
    self.EnableSetCompleteCheck = ko.observable(false);
    self.EnableManageFinance = ko.observable(false);
    self.EnableDelete = ko.observable(false);
    self.Id = ko.observable();
    self.Type = ko.observable();
    self.Name = ko.observable("").extend({ required: true });
    self.Consignor = ko.observable("").extend({ required: true });
    self.StartTime = ko.observable("").extend({ required: true });
    self.StartTime.subscribe(function (newValue) {
        $("#Deadline").datepicker("setStartDate", newValue || undefined);
    });
    self.Deadline = ko.observable("").extend({ required: true });
    self.Deadline.subscribe(function (newValue) {
        $("#StartTime").datepicker("setEndDate", newValue || undefined);
    });

    self.Status = ko.observable();
    self.ManagerIds = ko.observable();
    self.ManagerNames = ko.observable();
    self.ParticipantIds = ko.observable();
    self.ParticipantNames = ko.observable();
    self.NeedFinish = ko.observable();
    self.CurrentProgress = ko.observable();
    self.NeedSupport = ko.observable();
    self.AdvancePlan = ko.observable();
    self.FinanceCode = ko.observable();
    self.Amount = ko.observable();
    self.FormattedAmount = computedFormatPrice(self, "Amount");
    self.TypeOfPayment = ko.observable();
    self.NeedFinish = ko.observable();
    self.AmountReceivedStatus = ko.observable();
    self.AmountReceived = ko.observable();
    self.FormattedAmountReceived = computedFormatPrice(self, "AmountReceived");
    self.AmountRemained = ko.computed(function () {
        var result = self.Amount() - self.AmountReceived();
        return thousandthFormatted(result.toFixed(2));
    });
    self.InvoiceStatus = ko.observable();
    self.Attachments = ko.observableArray();
    self.Budgets = ko.observableArray([]);
    self.CommentPageList = new pageListViewModel();
    self.ChangeLogPageList = new pageListViewModel();
    self.EditCommentContent = ko.observableArray("");
    self.EditExpenditure = new editExpenditureViewModel();
    //self.EditRemark = ko.observable("");
    //self.save = function () {
    //    self.EditRemark("");
    //    $("#remarkDialog").modal("show");
    //};
    self.addAttachment = function () {
        self.EditCommentContent("");
        $("#attachmentDialog").modal("show");
    };
    self.startUploadAttachment = function () {
        $("#file_upload").uploadify("settings", 'formData', { 'projectId': self.Id() });
        $('#file_upload').uploadify('upload', '*');
        //request("/Project/AddComment", { projectId: self.Id(), content: self.EditCommentContent() }, function () {
        //$("#attachmentDialog").modal("hide");
        //});
    };
    self.AllAttachmentsChecked = ko.computed({
        read: function () {
            if (self.Attachments() && self.Attachments().length > 0) {
                var items = self.Attachments();
                for (var i = 0; i < items.length; i++) {
                    if (!items[i].Checked()) {
                        return false;
                    }
                }
                return true;
            } else {
                return false;
            }
        },
        write: function (value) {
            var items = self.Attachments();
            for (var i = 0; i < items.length; i++) {
                items[i].Checked(value);
            }
        },
        owner: self
    });
    self.downloadAttachments = function () {
        if (!self.Attachments() || self.Attachments().length == 0) {
            showMessage("无可以下载的附件");
        } else {
            var attachments = self.Attachments(),
                checkedIds = [],
                flag = true;
            for (var i = 0; i < attachments.length; i++) {
                if (attachments[i].Checked()) {
                    checkedIds.push(attachments[i].Id);
                }
            }
            if (checkedIds.length == 0) {
                showMessage("请选择需要下载的附件");
            } else {
                showPopMessage("压缩附件", "正在生成附件压缩文件，请稍候……", function () {
                    flag = false;
                });

                request("/Project/ZipAttachments", { ids: checkedIds }, function (result) {
                    if (flag) {
                        updatePopMessage("附件压缩文件生成成功，<a onclick='closePopMessage();' target='_blank' " +
                            "href='/Project/DownloadAttachmentZipFile?path=" + result + "'>点击下载</a>");
                    }
                });
            }
        }
    };
    self.cancelUploadAttachment = function () {
        $('#file_upload').uploadify('cancel', '*');
        $('#file_upload').uploadify('stop');
    };
    self.deleteAttachment = function (attachment) {
        showPopConfrimMessage("删除确认", "是否确认删除指定的附件？", function () {
            request("/Project/DeleteAttachment", { attachmentId: attachment.Id }, function () {
                self.refrushAttachments();
                showMessage("删除附件成功");
            });
        });
    };
    self.refrushAttachments = function () {
        request("/Project/LoadAttachments", {
            projectId: self.Id()
        }, function (result) {
            self.Attachments(makeCheckArray(result));
        });
    };
    self.addComment = function () {
        self.EditCommentContent("");
        $("#commentDialog").modal("show");
    };
    self.deleteComment = function (comment) {
        showPopConfrimMessage("删除确认", "是否确认删除指定的评论？", function () {

            request("/Project/DeleteComment", { commentId: comment.Id }, function () {
                self.refrushComments();
                showMessage("删除附件成功");
            });
        });
    };
    self.saveComment = function () {
        request("/Project/AddComment", { projectId: self.Id(), content: self.EditCommentContent() }, function () {
            $("#commentDialog").modal("hide");
            self.refrushComments();
        });
    };
    self.refrushComments = function () {
        request("/Project/LoadComments", {
            projectId: self.Id(),
            pageIndex: self.CommentPageList.PageIndex(),
            pageSize: self.CommentPageList.PageSize()
        }, function (result) {
            self.CommentPageList.update(result);
        });
    };
    self.nextPageComments = function () {
        var pageIndex = self.CommentPageList.PageIndex();
        if (pageIndex < self.CommentPageList.MaxPageIndex()) {
            self.CommentPageList.PageIndex(pageIndex + 1);
            self.refrushComments();
        }
    };
    self.prePageComments = function () {
        var pageIndex = self.CommentPageList.PageIndex();
        if (pageIndex > 1) {
            self.CommentPageList.PageIndex(pageIndex - 1);
            self.refrushComments();
        }
    };
    self.refrushChangeLogs = function () {
        self.CommentPageList.update();
        request("/Project/LoadProjectChangeLogs", {
            projectId: self.Id(),
            pageIndex: self.ChangeLogPageList.PageIndex(),
            pageSize: self.ChangeLogPageList.PageSize()
        }, function (result) {
            self.ChangeLogPageList.update(result);
        });
    };
    self.nextPageChangeLogs = function () {
        var pageIndex = self.ChangeLogPageList.PageIndex();
        if (pageIndex < self.ChangeLogPageList.MaxPageIndex()) {
            self.ChangeLogPageList.PageIndex(pageIndex + 1);
            self.refrushChangeLogs();
        }
    };
    self.prePageChangeLogs = function () {
        var pageIndex = self.ChangeLogPageList.PageIndex();
        if (pageIndex > 1) {
            self.ChangeLogPageList.PageIndex(pageIndex - 1);
            self.refrushChangeLogs();
        }
    };
    self.sendFinishCheck = function () {
        request("/Project/SendFinishCheck", { projectId: self.Id() }, function (result) {
            self.Status(result);
            showMessage("操作成功");
        });
    };
    self.refrushBudgets = function () {
        request("/Project/LoadBudgets", {
            projectId: self.Id()
        }, function (result) {
            self.updateBudgets(result);
        });
    };
    self.updateBudgets = function (budgets) {
        var models = [];
        if (budgets) {
            for (var i = 0; i < budgets.length; i++) {
                models.push(new projectBudgetViewModel(budgets[i]));
            }
        }
        self.Budgets(models);
    };
    self.editBudgets = function () {
        var budgets = self.Budgets();
        if (budgets) {
            for (var i = 0; i < budgets.length; i++) {
                budgets[i].editReset();
            }
        }
        $("#budgetsDialog").modal("show");
    };
    self.saveBudgets = function () {
        var budgets = self.Budgets(),
            totalAmount = 0,
            items = [],
            budget,
            amount = 0;
        for (var i = 0; i < budgets.length; i++) {
            budget = budgets[i];
            amount = budget.EditAmout();
            totalAmount = totalAmount + amount;
            if (amount < budget.Expended()) {
                showErrorMessage(budget.CategoryString() + "预算的金额不能小于已经支出的金额");
                return;
            }
            items.push({ Category: budget.Category(), Amount: amount });
        }
        if (totalAmount > self.Amount()) {
            showErrorMessage("预算的金额总和不能大于项目金额");
            return;
        }
        request("/Project/EditBudgets", {
            projectId: self.Id(),
            items: items
        }, function (result) {
            showMessage("预算编辑成功");
            self.refrushBudgets();
            $("#budgetsDialog").modal("hide");
        });
    };
    self.addExpenditure = function (budget) {
        self.EditExpenditure.setBudget(budget);
        $("#expenditureDialog").modal("show");
    };
    self.saveExpenditure = function () {
        var expenditure = self.EditExpenditure;
        if (expenditure.Count() > expenditure.BudgetRemained()) {
            showErrorMessage("支出金额不能超过该类预算的剩余金额");
            return;
        }
        request("/Project/AddExpenditure", {
            expenditure: self.EditExpenditure
        }, function (result) {
            showMessage("支出记录添加成功");
            self.refrushExpenditures(expenditure.BudgetCategory());
            $("#expenditureDialog").modal("hide");
        });
    };
    self.refrushExpenditures = function (category) {
        var budgets = self.Budgets();
        if (budgets) {
            for (var i = 0; i < budgets.length; i++) {
                if (budgets[i].Category() == category) {
                    budgets[i].refreshExpenditures();
                    return;
                }
            }
        }
    };
    self.update = function (project, onlyBaseInfo) {
        self.EnableEditProject(project.EnableEditProject);
        self.EnableSetCompleteCheck(project.EnableSetCompleteCheck);
        self.EnableDelete(project.EnableDelete);
        self.EnableManageFinance(project.EnableManageFinance);
        self.Id(project.Id);
        self.Type(project.Type);
        self.Name(project.Name);
        self.Consignor(project.Consignor);
        self.StartTime(formattedDateStr(project.StartTime));
        self.Deadline(formattedDateStr(project.Deadline));
        self.Status(project.Status);
        self.ManagerIds(project.ManagerIds);
        self.ManagerNames(project.ManagerNames);
        self.ParticipantIds(project.ParticipantIds);
        self.ParticipantNames(project.ParticipantNames);
        self.NeedFinish(project.NeedFinish);
        self.CurrentProgress(project.CurrentProgress);
        self.NeedSupport(project.NeedSupport);
        self.AdvancePlan(project.AdvancePlan);
        self.FinanceCode(project.FinanceCode);
        self.Amount(project.Amount);
        self.TypeOfPayment(project.TypeOfPayment);
        self.NeedFinish(project.NeedFinish);
        self.AmountReceivedStatus(project.AmountReceivedStatus);
        self.AmountReceived(project.AmountReceived);
        self.InvoiceStatus(project.InvoiceStatus);
        if (!onlyBaseInfo) {
            self.Attachments(makeCheckArray(project.Attachments));
            self.CommentPageList.update(project.CommentPageList);
            self.updateBudgets(project.Budgets);
        } else {
            var budgets = self.Budgets();
            //需要重新加载财务信息
            if (project.EnableManageFinance) {
                self.refrushBudgets();
            } else {
                self.updateBudgets([]);
            }
        }

    };

    $("#StartTime,#Deadline").datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        orientation: "top left",
        minView: "month",
        language: "zh-CN",
        todayBtn: "linked"
    });
    $('#file_upload').uploadify({
        'formData': { 'projectId': '' },
        'fileSizeLimit': '10MB',
        'successTimeout': 600,
        'uploadLimit': 100,
        'removeCompleted': true,
        'queueID': 'queueUpload',
        'swf': '/Scripts/uploadify.swf',
        'auto': false,
        'multi': true,
        'buttonClass': 'btn btn-primary',
        'buttonText': '选择文件',
        'uploader': '/Project/UploadAttachment',
        'onQueueComplete': function (queueData) {
            showMessage("上传附件完成");
            self.refrushAttachments();
        }
    });
    $(".uploadify-button,.uploadify").removeAttr("style");
    $("#attachmentDialog").on('hide.bs.modal', function () {
        self.cancelUploadAttachment();
    });
    $("#changelogs_tab").on('shown.bs.tab', function (e) {
        self.refrushChangeLogs();
    });
    self.update(project);
};

var projectBudgetViewModel = function (budget) {
    var self = this,
        priceFormattedExpenditures = function (expenditures) {
            if (expenditures && expenditures.length > 0) {
                for (var i = 0; i < expenditures.length; i++) {
                    expenditures[i].FormattedCount = thousandthFormatted(expenditures[i].Count.toFixed(2));
                }
            }
            return expenditures;
        };
    self.Id = ko.observable(budget.Id);
    self.ProjectId = ko.observable(budget.ProjectId);
    self.Category = ko.observable(budget.Category);
    self.CategoryString = ko.observable(budget.CategoryString);
    self.Amount = ko.observable(budget.Amount);
    self.EditAmout = ko.observable(budget.Amount);
    self.FormattedAmount = computedFormatPrice(self, "Amount");
    self.FormattedEditAmout = computedFormatPrice(self, "EditAmout");
    self.Expenditures = ko.observableArray(priceFormattedExpenditures(budget.Expenditures));
    self.Expended = ko.computed(function () {
        var result = 0,
            expenditures = self.Expenditures();
        if (expenditures && expenditures.length > 0) {
            for (var i = 0; i < expenditures.length; i++) {
                result += expenditures[i].Count;
            }
        }
        return result;
    });
    self.FormattedExpended = computedFormatPrice(self, "Expended");
    self.Remained = ko.computed(function () {
        var result = self.Amount() - self.Expended();
        return result;
    });
    self.FormattedRemained = computedFormatPrice(self, "Remained");
    self.ExpendituresVisible = ko.observable(false);

    self.toggleExpenditures = function () {
        self.ExpendituresVisible(!self.ExpendituresVisible());
    };
    self.deleteExpenditure = function (expenditure) {
        showPopConfrimMessage("删除确认", "是否确认删除指定的支出记录？", function () {
            request("/Project/DeleteExpenditure", {
                expenditureId: expenditure.Id
            }, function (result) {
                showMessage("删除支出记录成功");
                self.refreshExpenditures();
            });
        });
    };
    self.refreshExpenditures = function () {
        request("/Project/LoadExpenditure", {
            projectId: self.ProjectId(),
            category: self.Category()
        }, function (result) {
            self.Expenditures(priceFormattedExpenditures(result));
        });
    };
    self.editReset = function () {
        self.EditAmout(self.Amount());
    };
};

var editExpenditureViewModel = function () {
    var self = this;
    self.ProjectId = ko.observable();
    self.BudgetCategory = ko.observable();
    self.BudgetCategoryString = ko.observable();
    self.BudgetRemained = ko.observable();
    self.BudgetFormattedRemained = ko.observable();
    self.Count = ko.observable();
    self.FormattedCount = computedFormatPrice(self, "Count");
    self.Remark = ko.observable();
    self.setBudget = function (budget) {
        self.ProjectId(budget.ProjectId());
        self.BudgetCategory(budget.Category());
        self.BudgetCategoryString(budget.CategoryString());
        self.BudgetRemained(budget.Remained());
        self.BudgetFormattedRemained(budget.FormattedRemained());
        self.Count(0);
        self.Remark("");
    };
};

var projectListSearchViewModel = function () {
    var self = this,
        rangeComputed = function (fromField, toField) {
            return ko.computed(function () {
                var from = self[fromField]() || "";
                var to = self[toField]() || "";
                if (from || to) {
                    return from + "," + to;
                } else {
                    return "";
                }
            });
        };
    self.FilterFields = ["ProjectName", "ManagerId", "ParticipantId", "StartTime", "Deadline"];
    self.ProjectName = ko.observable();
    self.ManagerId = ko.observable();
    self.ParticipantId = ko.observable();
    self.StartTimeFrom = ko.observable();
    self.StartTimeTo = ko.observable();
    self.DeadlineFrom = ko.observable();
    self.DeadlineTo = ko.observable();
    self.StartTime = rangeComputed("StartTimeFrom", "StartTimeTo");;
    self.Deadline = rangeComputed("DeadlineFrom", "DeadlineTo");;
    self.clear = function () {
        for (var prop in self) {
            if (self.hasOwnProperty(prop) && ko.isObservable(self[prop])) {
                self[prop]("");
            }
        }
    };
    self.getFilters = function () {
        var filters = [];
        for (var i = 0; i < self.FilterFields.length; i++) {
            var field = self.FilterFields[i];
            if (self[field]()) {
                filters.push({ Field: field, Value: self[field]() });
            }
        }
        return filters;
    };
}

var projectListItemViewModel = function (project) {
    var self = this,
        init = function (project) {
            project.StartTime = new Date(Date.parse(project.StartTime));
            project.Deadline = new Date(Date.parse(project.Deadline));

            self.Checked = ko.observable(false);
            self.Id = project.Id;
            self.Name = project.Name;
            self.StartTime = formatDateString(project.StartTime);
            self.Deadline = formatDateString(project.Deadline);
            self.ManagerNames = project.ManagerNames;
            self.ParticipantNames = project.ParticipantNames;
            //self.creator = project.Creator;
            //self.createTime = formatDateString(project.CreateTime);
            self.Status = project.Status;
            self.DaySpanHtml = projectDaySpanHtml(project);
            self.LatestNews = project.LatestNews;
            self.HasNewsView = ko.observable(project.HasNewsView);
            self.EnableViewDetail = project.EnableViewDetail;
            self.EnableDelete = project.EnableDelete;
        },
        formatDateString = function (date) {
            return date.getFullYear() + "年" + (date.getMonth() + 1) + "月" + date.getDate() + "日";
        },
        projectDaySpanHtml = function (project) {
            var result = "",
                statusName = "";
            switch (project.Status) {
                case 0:
                    statusName = "发布待审";
                    break;
                case 1:
                    statusName = "进行中";
                    break;
                case 2:
                    statusName = "完成待审";
                    break;
                case 3:
                    statusName = "已完成";
                    break;
                case 4:
                    statusName = "已取消";
                    break;
            }
            result = '<small class="text-info">' + statusName + '</small>';

            if (project.Status != 3 && project.Status != 4) {
                var daySpan = parseInt((project.Deadline - new Date()) / 1000 / 60 / 60 / 24),
                    daySpanStr = daySpan,
                    daySpanClass,
                    prefix = "";
                if (daySpan < 0) {
                    daySpanStr = -daySpan;
                    daySpanClass = "timespan-expired";
                    prefix = "<small>过期</small>";
                }
                else if (daySpan == 0) {
                    daySpanStr = "今"
                    daySpanClass = "timespan-day";
                } else {
                    prefix = "<small>剩余</small>";
                    if (daySpan <= 3) {
                        daySpanClass = "timespan-day";
                    } else if (daySpan <= 7) {
                        daySpanClass = "timespan-week";
                    } else if (daySpan <= 30) {
                        daySpanClass = "timespan-month";
                    } else {
                        daySpanClass = "timespan";
                    }
                }
                result = prefix + '<span class="' + daySpanClass + '">' + daySpanStr + '</span><small>天</small>' + ' ' + result;
            }
            return result;
        };
    init(project);
    return self;
};

var projectListViewModel = function (user) {
    var self = window.viewModel = window.viewModel || {},
        init = function () {
            self.user = user;
            self.queryFilter = { FieldFilters: [], SortFields: [], PageIndex: 1, PageSize: 20 };
            self.searchCondition = new projectListSearchViewModel();
            self.pageList = new pageListViewModel();
            self.projectEdit = new ko.validatedObservable(new projectEditViewModel({}));
            self.projectEdit.isValid();
            self.changeMessages = new projectChangeMessagesViewModel();
            self.anyItemSelected = ko.computed(function () {
                var items = self.pageList.List();
                for (var i = 0; i < items.length; i++) {
                    if (items[i].Checked()) {
                        return true;
                    }
                }
                return false;
            });

            self.query = function () {
                query(true);
                showProjectListView();
            };
            self.search = function () {
                query(true);
            };
            self.refrush = function () {
                query();
            };
            self.nextPage = function () {
                var pageIndex = self.pageList.nextPage();
                if (pageIndex != -1) {
                    self.queryFilter.PageIndex = pageIndex;
                    query();
                }
            };
            self.prePage = function () {
                var pageIndex = self.pageList.prePage();
                if (pageIndex != -1) {
                    self.queryFilter.PageIndex = pageIndex;
                    query();
                }
            };
            self.back = function () {
                query();
                showProjectListView();
            };
            self.exportQuery = function () {
                var flag = true;
                showPopMessage("项目导出", "正在生成导出文件，请稍候……", function () {
                    flag = false;
                });
                request("/Project/ExportProjects", { queryFilter: self.queryFilter }, function (result) {
                    if (flag) {
                        updatePopMessage("导出文件生成成功，<a onclick='closePopMessage();' target='_blank' " +
                            "href='/Project/DownloadProjectExportFile?path=" + result + "'>点击下载</a>");
                    }
                });
            };
            self.add = function () {
                self.projectEdit().update({ EnableEditProject: true });
                showProjectEditView();
            };
            self.save = function () {
                if (self.projectEdit.isValid()) {
                    if (self.projectEdit().Id()) {
                        showPopConfrimMessage("修改备注", '<textarea id="editRemark" class="form-control" rows="4"></textarea>', function () {
                            var editRemark = $("#editRemark").val();
                            saveProject({ project: self.projectEdit, remark: editRemark });
                        });
                    } else {
                        saveProject({ project: self.projectEdit });
                    }
                }
            };
            self.edit = function (data) {
                if (data.EnableViewDetail) {
                    editeProject(data.Id);
                } else {
                    showErrorMessage("无该项目的查看权限");
                }

            };
            self.delete = function (data) {
                showPopConfrimMessage("删除确认", "是否确认删除指定的项目？", function () {
                    request("/Project/DeleteProject", { projectId: data.Id }, function (result) {
                        showMessage("删除成功");
                        showProjectListView();
                        query(false);
                    });
                });
            };
            self.check = function (status) {
                var projectIds = [];
                var projects = self.pageList.List();
                for (var i = 0; i < projects.length; i++) {
                    if (projects[i].Checked()) {
                        projectIds.push(projects[i].Id);
                    }
                }
                request("/Project/CheckProjects", { projectIds: projectIds, status: status }, function () {
                    self.refrush();
                    showMessage("操作成功");
                });
            };
            self.changeItemsChecked = function (checked) {
                var items = self.pageList.List();
                for (var i = 0; i < items.length; i++) {
                    items[i].Checked(checked);
                }
            };

            self.AllChecked = ko.computed({
                read: function () {
                    if (self.pageList.List().length > 0) {
                        var items = self.pageList.List();
                        for (var i = 0; i < items.length; i++) {
                            if (!items[i].Checked()) {
                                return false;
                            }
                        }
                        return true;
                    } else {
                        return false;
                    }
                },
                write: function (value) {
                    self.changeItemsChecked(value);
                },
                owner: self
            });

            self.removeChangeMessage = function (item) {
                request("/Project/UpdateProjectViewStatusRead", { projectId: item.ProjectId }, function (result) {
                    var projects = self.pageList.List();
                    self.changeMessages.messages.remove(item);
                    for (var i = 0; i < projects.length; i++) {
                        if (projects[i].Id == item.ProjectId) {
                            projects[i].HasNewsView(false);
                            break;
                        }
                    }
                });
            };

            self.removeAllChangeMessages = function () {
                request("/Project/UpdateAllProjectViewStatusRead", {}, function (result) {
                    self.changeMessages.removeAll();
                    var projects = self.pageList.List();
                    for (var i = 0; i < projects.length; i++) {
                        projects[i].HasNewsView(false);
                    }
                });
            };

            self.viewChangeMessage = function (item) {
                editeProject(item.ProjectId);
            };

            self.filterLinks = [
                new filterLinkViewModel("RelationType", "我的项目",
                    [
                        { value: "0", name: "我发布的" },
                        { value: "1", name: "我负责的" },
                        { value: "2", name: "我参与的" }
                    ], "", self.query),
                new filterLinkViewModel("Status", "项目状态",
                    [
                        { value: "1", name: "进行中" },
                        { value: "3", name: "已完成" },
                        { value: "0", name: "发布待审" },
                        { value: "2", name: "完成待审" }
                    ], "1", self.query),
                new filterLinkViewModel("Type", "项目类型",
                    [
                        { value: "0", name: "纵向研究" },
                        { value: "1", name: "横向研究" },
                        { value: "2", name: "横向咨询" },
                        { value: "3", name: "纵向工作" },
                        { value: "4", name: "中心工作" }
                    ], "", self.query)
            ];

            self.toggleMessageList = function () {
                $(".message-list").toggle();
                $(".message-header").find(".glyphicon").toggleClass("glyphicon-chevron-up glyphicon-minus");
            };
            query(true);
            self.changeMessages.start();
        },
        editeProject = function (projectId) {
            self.changeMessages.remove(projectId);
            request("/Project/GetProject", { projectId: projectId }, function (result) {
                self.projectEdit().update(result);
                showProjectEditView();
            });
        },
        saveProject = function (data) {
            request("/Project/EditProject", data, function (result) {
                self.projectEdit().update(result, true);
                showMessage("保存成功");
            });
        },
        updatePageList = function (pageList) {
            pageList.List = $.map(pageList.List, function (item) {
                return new projectListItemViewModel(item);
            });
            self.pageList.update(pageList);
        },
        query = function (updateQueryFilter) {
            if (updateQueryFilter) {
                var queryFilter = self.queryFilter;
                queryFilter.FieldFilters = self.searchCondition.getFilters();
                queryFilter.SortFields = [];
                for (var i = 0; i < self.filterLinks.length; i++) {
                    var filterLink = self.filterLinks[i];
                    queryFilter.FieldFilters.push({ Field: filterLink.field, Value: filterLink.value() });
                }

                queryFilter.PageIndex = 1;
                self.queryFilter = queryFilter;
            }

            request("/Project/LoadProjects", { queryFilter: self.queryFilter }, function (result) {
                updatePageList(result);
            });

        },
        showProjectEditView = function () {
            $("#list").hide();
            $("#listToolbar").hide();
            $("#searchPanel").hide();
            $("#edit").show();
            $("#editToolbar").show();
            $("#project_tab").tab("show");
            window.scrollTo(0, 0);
        },
        showProjectListView = function () {
            $("#list").show();
            $("#listToolbar").show();
            $("#edit").hide();
            $("#editToolbar").hide();
            window.scrollTo(0, 0);
        };

    $(".input-daterange").datepicker({
        format: "yyyy-mm-dd",
        autoclose: true,
        orientation: "top left",
        language: "zh-CN",
        todayBtn: "linked"
    });

    init();
};

var projectChangeMessagesViewModel = function () {
    var self = this;
    self.messages = ko.observableArray([]);
    self.total = ko.computed(function () {
        var messages = self.messages(),
            result = 0;
        for (var i = 0; i < messages.length; i++) {
            result = result + messages[i].Count;
        }
        return result;
    });
    self.start = function () {
        self.refrush();
        setInterval(self.refrush, 300000);
    };
    self.refrush = function () {
        request("/Project/LoadChangeMessages", {}, function (result) {
            self.messages(result);
        });
    };
    self.remove = function (projectId) {
        var removeItem = undefined,
            messages = self.messages();
        for (var i = 0; i < messages.length; i++) {
            if (messages[i].ProjectId == projectId) {
                removeItem = messages[i];
                break;
            }
        }
        if (removeItem) {
            self.messages.remove(removeItem);
        }
    };
    self.removeAll = function () {
        self.messages([]);
    };
};