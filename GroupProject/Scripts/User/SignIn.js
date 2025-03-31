$(document).ready(function () {

    function addNewUser() {
        var adminData = {
            Name: $('#name').val(),
            Email: $('#email').val(),
            Password: $('#password').val(),
            Role: $('#role').val(),
            Status: $('#status').val()
        };

        // Check if the username already exists
        $.ajax({
            url: '/SignIn/CheckUserName',
            type: 'GET',
            data: { name: adminData.Name },
            success: function (response) {
                console.log('Response from server:', response);  // Debugging log to inspect response
                if (response) {
                    alert('Username already exists. Please choose a different name.');
                } else {
                    // If username doesn't exist, send the POST request to create the admin user
                    $.ajax({
                        url: '/SignIn/AddUser',
                        type: 'POST',
                        data: adminData,
                        success: function (response) {
                            if (response.success) {
                                Swal.fire('Success!', 'Admin Account Created Successfully!', 'success');

                            } else {
                                Swal.fire('Error!', 'Failed to Create An Account', 'error');

                            }
                        },
                        error: function () {
                            Swal.fire('Error!', 'An error occurred: ' + error, 'error');
                        }
                    });
                }
            },
            error: function () {
                Swal.fire('Error!', 'An error occurred: ' + error, 'error');
            }
        });
    }

    $('#submitUser').click(addNewUser);


})