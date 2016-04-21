
var ZipCodeMethod = {
    form: false,
    saveUrl: false,
    zipCodeElementId: "",
    messageElementId:"",

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
        
    },

    validate: function() {
        var IsValid = true;
        var elementId = $("#request-method-message");
        elementId.html("");
        
        var zipcodeField = $("#" + this.zipCodeElementId).val();

        if (zipcodeField != "") {
            var isValidZip = /(^\d{5}$)|(^\d{5}-\d{4}$)/.test(zipcodeField);

            if (isValidZip == false) {
                elementId.append(" Zipcode is not valid.");
                IsValid = false;
            }
            
        }
        else {
            IsValid = false;
            elementId.append(" Zipcode is required.");
        }

        if (IsValid == false) {
            elementId.addClass('text-danger');
            elementId.removeClass('text-success');
            elementId.show();
        }
        else {           
            elementId.removeClass('text-danger');
            elementId.hide();
        }
        return IsValid;
    },
    
    CheckZipCodeAvailability: function (zipCodeElementId,messageElementId) {
        this.zipCodeElementId = zipCodeElementId;
        this.messageElementId = messageElementId;
        //alert(this.messageElementId);
       if (this.validate()) {
           $.ajax({
                    async :false,
                    cache: false,
                    url: this.saveUrl,
                    data: { inputzipcode: $("#" + this.zipCodeElementId).val(), messageElementId: this.messageElementId },
                    type: 'post',
                    success: function (response) {
                        if (response.Result == true) {
                            var elementId = $("#" + response.messageElementId);
                            elementId.addClass('text-success');
                            elementId.removeClass('text-danger');
                            elementId.show();
                            elementId.html(response.message);

                        }
                        else {
                            var elementId = $("#" + response.messageElementId);
                            elementId.addClass('text-danger');
                            elementId.removeClass('text-success');
                            elementId.show();
                            elementId.html(response.message);
                        }

                        return false;
                    },
                    complete: this.resetLoadWaiting,                   
                });          
        }
    },

    resetLoadWaiting: function () {
        
    },


    nextStep: function (response, messageElementId) {


       
        if (response.Result == true) {


        }
        else {
            
            var elementId = $("#" + this.messageElementId);
            elementId.addClass('text-danger');
            elementId.removeClass('text-success');
            elementId.show();
            elementId.html(response.message);
        }

        return false;
    },
    
};
