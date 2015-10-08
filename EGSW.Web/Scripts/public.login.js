var Checkout = {
    loadWaiting: false,
    failureUrl: false,

    init: function (failureUrl) {
        this.loadWaiting = false;
        this.failureUrl = failureUrl;
    },

    ajaxFailure: function () {
        location.href = Checkout.failureUrl;
    },   
   
    _disableEnableAll: function (element, isDisabled) {
        var descendants = element.find('*');
        $(descendants).each(function () {
            if (isDisabled) {
                $(this).attr('disabled', 'disabled');
            } else {
                $(this).removeAttr('disabled');
            }
        });

        if (isDisabled) {
            element.attr('disabled', 'disabled');
        } else {
            element.removeAttr('disabled');
        }
    },


    setLoadWaiting: function (step, keepDisabled) {
        if (step) {
            if (this.loadWaiting) {
                this.setLoadWaiting(false);
            }
            var container = $('#' + step + '-buttons-container');
            container.addClass('disabled');
            container.css('opacity', '.5');
            this._disableEnableAll(container, true);
            $('#' + step + '-please-wait').show();
        } else {
            if (this.loadWaiting) {
                var container = $('#' + this.loadWaiting + '-buttons-container');
                var isDisabled = (keepDisabled ? true : false);
                if (!isDisabled) {
                    container.removeClass('disabled');
                    container.css('opacity', '1');
                }
                this._disableEnableAll(container, isDisabled);
                $('#' + this.loadWaiting + '-please-wait').hide();
            }
        }
        this.loadWaiting = step;
    },

    setStepResponse: function (response) {
        if (response.update_section) {
            $('#' + response.update_section.name + '-load').html(response.update_section.html);
        }       
        
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    }
};



var LoginMethod = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
    },

    validate: function() {
        var methods = document.getElementsByName('shippingoption');
        if (methods.length==0) {
            alert('Your order cannot be completed at this time as there is no shipping methods available for it. Please make necessary changes in your shipping address.');
            return false;
        }

        for (var i = 0; i< methods.length; i++) {
            if (methods[i].checked) {
                return true;
            }
        }
        alert('Please specify shipping method.');
        return false;
    },
    
    Login: function () {
        if (Checkout.loadWaiting != false) return;
        
        //if (this.validate()) {
            if (true) {
            Checkout.setLoadWaiting('login-method');
        
            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: { AjaxEmail: $('#AjaxEmail').val(), AjaxPassword: $('#AjaxPassword').val() },
                type: 'post',
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if ((typeof response.message) == 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
    }
};



var ServiceRequestMethod = {   
    saveUrl: false,
    showElementId: "request-method-message",

    init: function (showElementId, saveUrl) {
        this.showElementId = showElementId;
        this.saveUrl = saveUrl;
    },
    
    SendRequest: function () {
        
        
        if (true) {
            Checkout.setLoadWaiting('request-method');

            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: { ServiceEmailAdddress: $('#ServiceEmailAdddress').val(), ServiceZipCode: $('#ServiceZipCode').val() },
                type: 'post',
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
                
        var elementId = $("#request-method-message");
        elementId.show();
        if (response.Result) {            
            elementId.addClass('text-success');
            elementId.removeClass('text-danger');
            elementId.html("Your enquiry has been successfully sent to the site owner.");
            
        }
        else {
            elementId.html(response.Message);
            elementId.addClass('text-danger');
            elementId.removeClass('text-success');
        }
        $("#request-method-please-wait").hide();
        return false;       
    }
};
