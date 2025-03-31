$(document).ready(function () {

    //Get and Display users on the page
    fetchUsers();

    //on search btn click
    $('#searchButton').on('click', function () {
        fetchUsers();
    })

    //Get users based on the selected role and search keyword 
    function fetchUsers() {
        var userType = $('#userType').val();
        var searchQuery = $('#searchUser').val().trim();
        console.log("Searching for: ", searchQuery);
        $.ajax({
            // Controller action to fetch data (UserController)
            url: '/UserManagement/GetUsers',
            type: 'GET',
            data: {
                userType: userType,
                searchQuery: searchQuery
            },
            success: function (data) {
                //clear the table 
                $('#userTable tbody').empty();

                //check for data
                if (data.length > 0) {
                    //loop through and create rows 
                    data.forEach(function (user) {
                        var row = `<tr data-id="${user.Id}">
                        <td>${user.Name}</td>
                        <td>${user.Role}</td>
                        <td>${user.Email}</td>
                        <td>${user.Status}</td>
                        <td>
                             <button class="banBtn" data-id="${user.Id}" data-status="${user.Status}">Ban</button>  
                            <button class="unBanBtn" data-id="${user.Id}" data-status="${user.Status}">UnBan</button>  
                        </td>
                    </tr>`;
                        $('#userTable tbody').append(row);
                    });
                    // event listeners for ban/unban buttons
                    $('.banBtn').click(function () {
                        var userId = $(this).data('id');
                        var currentStatus = $(this).data('status');
                        if (currentStatus !== "Banned") {
                            confirmAction('Ban', userId, 'Banned');
                        } else {
                            Swal.fire('Already Banned', 'This user is already banned.', 'info');
                        }
                    });

                    $('.unBanBtn').click(function () {
                        var userId = $(this).data('id');
                        var currentStatus = $(this).data('status');
                        if (currentStatus === "Banned") {
                            confirmAction('UnBan', userId, 'Enter');
                        } else {
                            Swal.fire('Already Active', 'This user is already active.', 'info');
                        }
                    });
                }
                else {
                    // Display no user found message
                    $('#userTable tbody').append('<tr><td colspan="3">No user found</td></tr>');
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching users: " + error);
            }
        })
    }
    //add new admin acc
    function addNewAdmin() {
        var adminData = {
            Name: $('#name').val(),
            Email: $('#email').val(),
            Password: $('#password').val(),
            Role: $('#role').val(),
            Status: $('#status').val()
        };

        // Check if the username already exists
        $.ajax({
            url: '/UserManagement/CheckUserName',
            type: 'GET',
            data: { name: adminData.Name },
            success: function (response) {
                console.log('Response from server:', response);  // Debugging log to inspect response
                if (response) {
                    alert('Username already exists. Please choose a different name.');
                } else {
                    // If username doesn't exist, send the POST request to create the admin user
                    $.ajax({
                        url: '/UserManagement/AddAdmin',
                        type: 'POST',
                        data: adminData,
                        success: function (response) {
                            if (response.success) {
                                Swal.fire('Success!', 'Admin Account Created Successfully!', 'success');
                                
                            } else {
                                Swal.fire('Error!', 'Failed to add Admin.', 'error');
                                
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

    $('#submitUser').click(addNewAdmin);

    // Function to confirm Ban/UnBan action using Swal
    function confirmAction(action, userId, newStatus) {
        Swal.fire({
            title: `Are you sure you want to ${action} this user?`,
            text: `This action will change the user status to ${newStatus}.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Proceed',
            cancelButtonText: 'Cancel',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                // Proceed with updating the user status
                updateUserStatus(userId, newStatus);
            }
        });
    }

    // Function to update the user status in the database
    function updateUserStatus(userId, newStatus) {
        $.ajax({
            // Controller action to update the user status
            url: '/UserManagement/UpdateUserStatus',  
            type: 'POST',
            data: { userId: userId, status: newStatus },
            success: function (response) {
                if (response.success) {
                    Swal.fire('Success!', `User status updated to ${newStatus}.`, 'success');
                    fetchUsers(); // Refresh the user table
                } else {
                    Swal.fire('Error', 'There was a problem updating the user status.', 'error');
                }
            },
            error: function () {
                Swal.fire('Error', 'Error updating user status. Please try again.', 'error');
            }
        });
    }

})