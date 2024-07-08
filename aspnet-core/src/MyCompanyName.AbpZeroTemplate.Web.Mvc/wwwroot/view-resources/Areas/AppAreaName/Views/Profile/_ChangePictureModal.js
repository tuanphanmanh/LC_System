(function ($) {
    app.modals.ChangeProfilePictureModal = function () {
        var _modalManager;
        var $cropperJsApi = null;

        var _profileService = abp.services.app.profile;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            $('#ProfilePictureResize').hide();

            $('#Profile_UseGravatarProfilePicture').change(function () {
                var useGravatarProfilePicture = $(this).is(':checked');
                var $modal = _modalManager.getModal();

                if (useGravatarProfilePicture) {
                    $('[name="ProfilePicture"]').attr('disabled', 'disabled');
                    $modal.find('.cropperjs-active').hide();
                } else {
                    $('[name="ProfilePicture"]').removeAttr('disabled');
                    $modal.find('.cropperjs-active').show();
                }
            });
        };

        this.save = function () {
            var input = {};
            var useGravatarProfilePicture = $('#Profile_UseGravatarProfilePicture').is(':checked');

            if (useGravatarProfilePicture) {
                input.useGravatarProfilePicture = useGravatarProfilePicture;
                saveInternal(input);
                return;
            }

            //not gravatar
            var $fileInput = $('#ChangeProfilePictureModalForm input[name=ProfilePicture]');
            var files = $fileInput.get()[0].files;

            if (!files.length) {
                abp.notify.warn(app.localize('PleaseSelectAPicture'));
                return;
            }

            var file = files[0];

            //File type check
            var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
            if ('|jpg|jpeg|png|gif|'.indexOf(type) === -1) {
                abp.message.warn(app.localize('ProfilePicture_Warn_FileType'));
                return false;
            }

            //File size check
            if (file.size > 5242880) {
                //5MB
                abp.message.warn(app.localize('ProfilePicture_Warn_SizeLimit', app.consts.maxProfilePictureBytesUserFriendlyValue));
                return false;
            }

            saveCroppedImage((fileToken) => {
                input = {
                    fileToken: fileToken
                };

                var userIdInput = _modalManager.getModal().find("#userId");
                if (userIdInput.length === 1) {
                    input.userId = userIdInput.val();
                }

                saveInternal(input);
            });
        };

        function saveInternal(input) {
            _profileService.updateProfilePicture(input).done(function () {
                if ($cropperJsApi) {
                    $cropperJsApi = null;
                }

                $('.header-profile-picture').attr('src', app.getUserProfilePicturePath());
                _modalManager.close();
            });
        }

        $('#ProfilePicture').change(function () {
            var fileName = app.localize('ChooseAFile');
            if (this.files && this.files[0]) {
                fileName = this.files[0].name;

                var $profilePictureResize = $('#ProfilePictureResize');

                var fr = new FileReader();
                fr.onload = function (e) {
                    $profilePictureResize.attr('src', this.result);
                    $cropperJsApi = $profilePictureResize.cropper({
                        aspectRatio: 1,
                        viewMode: 1,
                    });
                };

                fr.readAsDataURL(this.files[0]);
            }

            $('#ProfilePictureLabel').text(fileName);
        });

        function saveCroppedImage(onSuccess) {
            $cropperJsApi.cropper('getCroppedCanvas').toBlob(function (blob) {
                var token = app.guid();
                
                var formData = new FormData();
                formData.append('ProfilePicture', blob);
                formData.append('FileToken', token);
                formData.append('FileName', "ProfilePicture");
                
                $.ajax('/Profile/UploadProfilePicture', {
                    method: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        onSuccess(token);
                    },
                    error: function (response) {
                        abp.message.error(response.error.message);
                    }
                });
            });
        }
    };
})(jQuery);
