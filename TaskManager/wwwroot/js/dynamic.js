// addNotifications to notification dropdonw menu
function AddNotification(notifications) {

    const $ele = $("#notification");

    $ele.empty(); // Clear existing notifications

    $ele.append('<h6 class="dropdown-header bg-gradient-primary text-white">Notifications</h6>');

    if (notifications && notifications.length > 0) {
        // Build the notification items as a single string for better performance
        let itemsHtml = notifications.map(n =>
            `
                <a class="dropdown-item notification-item d-flex align-items-center p-2 ${n.isRead == false ? 'bg-gray-200' : ""}" href="/Notification/GetNotification?id=${n.id}">
                    <div class="mr-3">
                        <div class="icon-circle bg-gradient-primary">
                            <i class="fas fa-tasks text-white"></i>
                        </div>
                    </div>
                    <div>
                        <div class="small text-gray-700 fw-bold">${n.createdAt}</div>
                        <span class="font-weight-bold">${n.message}</span>
                    </div>
                </a>
            `
        ).join('');

        // Add "Show All" link
        if (notifications.length >= 6) {
            itemsHtml += `
                    <a class="dropdown-item text-center small text-gray-500" href="/Notification/AllNotifications">
                        Show All
                    </a>`;
        }

        $ele.append(itemsHtml);

    } else {
        $ele.append('<p class="p-2">Empty</p>');
    }

}

// update notification badge count
function updateNotificationBadge() {
    $.get("/Notification/GetNotificationsData", function (data) {

            const $badge = $("#badge");

            // Clear existing notifications to prevent duplicates

            const { unRead } = data;

            // Update badge if there are unread notifications
            if (unRead && unRead.length > 0) {
                $badge.text(unRead.length);
            } else {

                $badge.empty();
                clearInterval(timei);
            }
    });
}

updateNotificationBadge(); // Initial call to set the badge count
var timei = setInterval(updateNotificationBadge, 1000); // Update every minute)


// trigger when notification menu is open
$('#alertsDropdown').on('shown.bs.dropdown', function (e) {
    e.preventDefault();

    const url = $(this).attr("href");
    const $badge = $("#badge");

    // Fetch notifications
    $.get("/Notification/GetNotificationsData")
        .done(function (data) {

            // display notifications in the dropdown
            AddNotification(data.all);

            if (data.unRead && data.unRead.length > 0) {
                markNotificationsAsRead(url, data.unRead, $badge);
            }
        })
        .fail(function (xhr) {
            console.error("Failed to fetch notifications:", xhr.responseText);
        });
})

// update notification's IsSeen status i.e. make IsSeen to True
function markNotificationsAsRead(url, unReadNotifications, $badge) {
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(unReadNotifications)
    })
    .done(function (r) {
        // Clear the badge after successfully marking as read
        $badge.empty();
    })
    .fail(function (xhr) {
        console.error("Failed to mark notifications as read:", xhr.responseText);
    });
}


// dynamically show the remaining task end time
var $timer = $(".timeLeft");
var dueDateStr = $timer.data("duedate");
var dueDate = new Date(dueDateStr);

function updateCountdown() {
    var now = new Date();
    var diff = dueDate - now;

    if (diff <= 0) {
        $timer.text("Expire");
        clearInterval(timerInterval);
        return;
    }

    var seconds = Math.floor((diff / 1000) % 60);
    var minutes = Math.floor((diff / (1000 * 60)) % 60);
    var hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
    var days = Math.floor(diff / (1000 * 60 * 60 * 24));

    $timer.text(`${days}d ${hours}h ${minutes}m ${seconds}s`);
}

updateCountdown(); // initial display
var timerInterval = setInterval(updateCountdown, 1000);


// opne addTask modal

$(document).on('click','#addTask', function () {
    var currentUrl = encodeURIComponent(window.location.pathname + window.location.search);
    $('#mainModal .modal-content').load('/Task/AddTask?returnUrl=' + currentUrl , function () {
        $('#mainModal').modal('show');
    });
});


// open editTask modal

$(document).on('click', '.edit-task', function () {
    var taskId = $(this).data('id');
    var currentUrl = encodeURIComponent(window.location.pathname + window.location.search);
    $('#mainModal .modal-content').load('/Task/EditTask?taskId=' + taskId + '&returnUrl=' + currentUrl, function () {
        $('#mainModal').modal('show');
    });
});

// open deleteTask modal

$(document).on('click', '.delete-task', function () {
    var taskId = $(this).data('id');
    var currentUrl = encodeURIComponent(window.location.pathname + window.location.search);
    $('#mainModal .modal-content').load('/Task/DeleteTask?taskId=' + taskId + '&returnUrl=' + currentUrl, function () {
         $('#mainModal').modal('show');
    });
});