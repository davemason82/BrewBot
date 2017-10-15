function Order(data) {
    this.TableNumber = data.tableNumber;
    this.OrderText = data.orderText;
    this.MannersUsed = data.mannersUsed;
    this.RequestingUser = data.requestingUser;
    this.Confirmed = data.confirmed;
}

function OrdersViewModel() {
    var self = this;
    self.orders = ko.observableArray();
    self.showOrders = ko.observable();
    self.showError = ko.computed(function () {
        return !self.showOrders();
    });
    self.errorMessage = ko.observable();

    self.loadOrders = function () {
        $.getJSON("/api/orders", function (allData) {
            var mappedData = $.map(allData, function (item) { return new Order(item) });
            self.orders(mappedData);
            self.showOrders(true);
        }).fail(function (jqxhr) {
            self.showOrders(false);
            self.errorMessage("Error occurred loading orders.");
        });
    };

    self.completeOrder = function (order) {
        $.ajax({
            url: '/api/orders?tableNumber=' + order.TableNumber + '&cancelled=false',
            type: 'DELETE',
            success: function (result) {
                alert('Order confirmed');
                self.orders.remove(order);
            },
            error: function () {
                alert('Failed to confirm order');
            }
        });
    };

    self.cancelOrder = function (order) {
        $.ajax({
            url: '/api/orders?tableNumber=' + order.TableNumber + '&cancelled=true',
            type: 'DELETE',
            success: function (result) {
                alert('Order cancelled');
                self.orders.remove(order);
            },
            error: function () {
                alert('Failed to cancel order');
            }
        });
    };

    self.loadOrders();

    setInterval(self.loadOrders, 10000);
}

var ordersViewModel = new OrdersViewModel();
ko.applyBindings(ordersViewModel);