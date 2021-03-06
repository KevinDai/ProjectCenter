﻿(function ($) {

    ko.validation.init({
        registerExtenders: true,
        messagesOnModified: false,
        errorsAsTitle: false,            // enables/disables showing of errors as title attribute of the target element.
        errorsAsTitleOnModified: false, // shows the error when hovering the input field (decorateElement must be true)
        messageTemplate: null,
        insertMessages: false,           // automatically inserts validation messages as <span></span>
        parseInputAttributes: false,    // parses the HTML5 validation attribute from a form element and adds that to the object
        writeInputAttributes: false,    // adds HTML5 input validation attributes to form elements that ko observable's are bound to
        decorateElement: true,         // false to keep backward compatibility
        errorClass: null,               // single class for error message and element
        errorElementClass: 'error',  // class to decorate error element
        errorMessageClass: 'validationMessage',  // class to decorate error message
        grouping: {
            deep: true,        //by default grouping is shallow
            observable: true    //and using observables
        }
    });

    $._messengerDefaults = {
        parentLocations: ['html'],
        extraClasses: 'messenger-fixed messenger-theme-block messenger-on-top'
    };

    window.filterLinkViewModel = function (field, name, selectListItems, defaultValue, changeCallback) {
        var self = this;
        self.field = field;
        self.name = name;
        self.selectListItems = $.map(selectListItems, function (item) {
            return new filterLinkItemViewModel(item);
        });
        self.value = ko.observable(defaultValue);
        self.select = function (data) {
            self.value(data.value);
            changeCallback();
        };
        self.clear = function () {
            self.value("");
            changeCallback();
        };

    };

    function filterLinkItemViewModel(selectListItem) {
        var self = this;
        self.name = selectListItem.name;
        self.value = selectListItem.value;
        self.count = ko.observable("");
    };

    window.pageListViewModel = function (pageList) {
        var self = this;
        self.List = ko.observableArray([]);
        self.PageIndex = ko.observable(0);
        self.PageSize = ko.observable(0);
        self.Total = ko.observable(0);
        self.MaxPageIndex = ko.computed(function () {
            var pageSize = self.PageSize();
            return pageSize == 0 ? 0 : self.Total() / pageSize;
        });
        self.StartIndex = ko.computed(function () {
            return (self.PageIndex() - 1) * self.PageSize() + 1
        });
        self.EndIndex = ko.computed(function () {
            return self.PageIndex() * self.PageSize() > self.Total() ? self.Total() : self.PageIndex() * self.PageSize()
        });
        self.nextPage = function () {
            var pageIndex = self.PageIndex();
            if (pageIndex < self.MaxPageIndex()) {
                return pageIndex + 1;
            }
            return -1;
        };
        self.prePage = function () {
            var pageIndex = self.PageIndex();
            if (pageIndex > 1) {
                return pageIndex - 1;
            }
            return -1;
        };
        self.update = function (pageList) {
            pageList = pageList || { List: [], PageIndex: 1, PageSize: 20, Total: 0 };
            self.List(pageList.List);
            self.PageIndex(pageList.PageIndex);
            self.PageSize(pageList.PageSize);
            self.Total(pageList.Total);
        };

        self.update(pageList);
    };
    //window.requestMessager = $.globalMessenger().post({
    //    message: "You'll never see me",
    //    id: '4',
    //    singleton: true
    //});
    //window.requestMessager = $.globalMessenger().post({ hideAfter: 5 }).hide();
    //requestMessager.hide();
    window.request = function (url, data, callback) {
        var temp = setTimeout('showLoadding("处理数据中…")', 1000 * 1);
        $.ajax({
            type: "POST",
            timeout: 600000,
            contentType: "application/json",
            url: url,
            data: ko.toJSON(data) /*JSON.stringify()*/
        }).success(function (postResult) {
            if (postResult && postResult.Status == 0) {
                callback(postResult.Data);
            } else {
                showErrorMessage(postResult.Message || "系统错误");
            }
            //alert("1");
        }).error(function (error) {
            showErrorMessage("系统错误");
        }).complete(function () {
            clearTimeout(temp);
            hideLoadding();
        });
    };

    var loaddingMessage = undefined;
    window.showLoadding = function (msg) {
        //hideALlMessage();
        if (loaddingMessage) {
            loaddingMessage.update(msg);
        } else {
            loaddingMessage = $.globalMessenger().post({
                message: msg,
                hideAfter: 600
            });
        };
    };

    window.hideLoadding = function () {
        if (loaddingMessage) {
            loaddingMessage.hide();
        }
    };

    var errorMessage = undefined;
    window.showErrorMessage = function (msg) {
        //hideALlMessage();
        if (errorMessage) {
            errorMessage.update(msg);
        } else {
            errorMessage = $.globalMessenger().post({
                message: msg,
                type: 'error',
                hideAfter: 3
            });
        }
    };

    var commonMessage = undefined;
    window.showMessage = function (msg) {
        //hideALlMessage();
        if (commonMessage) {
            commonMessage.update(msg);
        } else {
            commonMessage = $.globalMessenger().post({
                message: msg,
                hideAfter: 3
            });
        }
    };

    //var hideALlMessage = function () {
    //    $.globalMessenger().hideAll();
    //};

    //$.fn.popMessage = function (option, _relatedTarget) {
    //    return this.each(function () {
    //        var $this = $(this)
    //        var data = $this.data('popMessage')
    //        var options = $.extend({}, typeof option == 'object' && option)

    //        if (!data) $this.data('popMessage', (data = new PopMessage(this, options)))
    //        if (typeof option == 'string') data[option](_relatedTarget)
    //        else if (options.show) data.show(_relatedTarget)
    //    })
    //}

    var UserSelectBox = function (element, options) {
        var self = this;
        self.options = options ? $.extend(true, {}, options) : {};
        self.element = $(element);
        self.container = $("<div></div>");
        self.inputContainer = $('<div class="input-group"></div>');
        self.usersPanel = $('<div class="panel panel-default panel-user-select" style="display: none;"></div>');
        self.mouseover = false;
        self.init = function () {
            self.container.insertAfter(self.element);
            self.container.append(self.inputContainer);
            self.container.append(self.usersPanel);

            self.inputContainer.append(self.element);
            if (!options.bindValue
                || (self.valueElement = $("#" + options.bindValue)).length == 0) {
                self.valueElement = $('<input type="hidden" />');
                self.inputContainer.append(self.valueElement);
            }
            self.inputContainer.append($('<span class="input-group-btn"></span>').append(
                $('<a class="btn btn-default" href="javascript:void(0);"><span class="glyphicon glyphicon-chevron-down"></span></a>').click(function () {
                    if ($(self.usersPanel).is(":visible")) {
                        self.element.blur();
                        self.hide();
                    } else {
                        self.element.focus();
                        self.show();
                    }
                })));
            var usersRow = $('<div class="row">').appendTo(self.usersPanel);
            for (var i = 0; i < options.users.length; i++) {
                usersRow.append('<div class="col-md-3"><div class="checkbox"><label><input value="' + options.users[i].Value + '" type="checkbox"><span>' + options.users[i].Text + '</span></label></div>')
            }
            self.usersPanel.find(":checkbox").click(function () {
                var ids = [];
                var names = [];
                self.usersPanel.find(":checkbox").each(function () {
                    if (this.checked) {
                        var $this = $(this);
                        ids.push($this.val());
                        names.push($this.next().text());
                    }
                });
                self.valueElement.val(ids ? "&" + ids.join("&") : "");
                self.element.val(names.join(","));
                self.element.trigger("change");
                self.valueElement.trigger("change");
            });
            self.element.keydown(function () {
                return false;
            });
            self.element.focus(function () {
                if (!$(self.usersPanel).is(":visible")) {
                    self.show();
                }
            });
            self.element.blur(function (event) {
                if (self.mouseover) {
                    self.element.focus();
                } else {
                    self.hide();
                }
                return true;
            });

            self.container.mouseover(function () {
                self.mouseover = true;
            });
            self.container.mouseout(function () {
                self.mouseover = false;
            });

        }

        self.show = function () {
            self.clearChecked();

            if (self.valueElement.val()) {
                var value = self.valueElement.val();
                if (value) {
                    self.check(value ? value.replace("&", "").split("&") : []);
                }
            }

            $(self.inputContainer).find(".glyphicon").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
            self.usersPanel.show();
        };
        self.hide = function () {
            $(self.inputContainer).find(".glyphicon").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
            self.usersPanel.hide();
        };

        self.check = function (ids) {
            self.clearChecked();
            if (ids) {
                self.usersPanel.find("input:checkbox").each(function () {
                    var $this = $(this);
                    for (var i = 0; i < ids.length; i++) {
                        if (ids[i] == $this.val()) {
                            this.checked = true;
                            break;
                        }
                    }
                });
            }
        };
        self.clearChecked = function () {
            self.usersPanel.find("input:checkbox").each(function () {
                this.checked = false;
            });
        };
        self.init();
    }

    $.fn.userSelectBox = function (options) {
        this.each(function () {
            new UserSelectBox(this, options);
        });
    }


    $(function () {
        (function () {
            var $element = $("#popMessage"),
                $btnOk = $element.find(".btn-ok"),
                okCallback = undefined,
                cancelCallback = undefined,
                init = function () {
                    $element.on('hide.bs.modal', function () {
                        if (cancelCallback) cancelCallback();
                        reset();
                    });
                    $btnOk.click(function () {
                        if (okCallback) okCallback();
                        hide();
                    });
                },
                reset = function () {
                    setTitle("");
                    setContent("");
                    okCallback = undefined;
                    cancelCallback = undefined;
                },
                setTitle = function (title) {
                    $element.find(".title").text(title);
                },
                setContent = function (content) {
                    $element.find(".content").html(content);
                },
                show = function (showBtnOk) {
                    if (showBtnOk) {
                        $btnOk.show();
                    } else {
                        $btnOk.hide();
                    }
                    $element.modal("show");
                },
                hide = function () {
                    $element.modal("hide");
                };

            init();

            window.showPopMessage = function (title, message, onCancel) {
                setTitle(title);
                setContent(message);
                cancelCallback = onCancel;
                show(false);
            };

            window.updatePopMessage = function (message) {
                setContent(message);
            };

            window.closePopMessage = function () {
                hide();
            };

            window.showPopConfrimMessage = function (title, message, onOk, onCancel) {
                setTitle(title);
                setContent(message);
                okCallback = onOk;
                cancelCallback = onCancel;
                show(true);
            };

        })();
    });

    var passwordEditViewModel = function () {
        var self = this;
        self.Password = ko.observable("");
        self.PasswordConfrim = ko.observable("");
        self.edit = function () {
            self.Password("");
            self.PasswordConfrim("");
            $("#passwordEdit").modal("show");
        };
        self.save = function () {
            var password = self.Password();
            var confrim = self.PasswordConfrim();
            if (password.length < 6 || password.length > 12) {
                showErrorMessage("密码长度必须在6到12个字符之间");
                return;
            }
            if (password != confrim) {
                showErrorMessage("设置的新密码必须和确认密码一致");
                return;
            }
            request("/User/ChangePassword", { password: password }, function (result) {
                showMessage("更新密码成功，下次登录请使用新密码登录");
                $("#passwordEdit").modal("hide");
            });
        };
    };

    window.viewModel = {
        passwordEdit: new passwordEditViewModel()
    };

})(jQuery);