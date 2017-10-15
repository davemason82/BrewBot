using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;
using ZutoBrewBot.Repositories.Interfaces;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;

namespace ZutoBrewBot.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private IAmazonDynamoDB _client;
        private readonly string _tableName = "zutobrewbot_orders";
        
        public OrderRepository()
        {
            // This is a service locator hack, but the AWS docs method for using DI isn't working
            var options = Startup.Configuration.GetAWSOptions();
            _client = options.CreateServiceClient<IAmazonDynamoDB>();
        }

        public void Insert(Order order)
        {
            var brewOrder = GetDocument(order);
            _client.PutItemAsync(
                tableName: _tableName,
                item: GetDocument(order)
            ).Wait();
        }

        public void Delete(int tableNumber)
        {
            _client.DeleteItemAsync(
                tableName: _tableName,
                key: new Dictionary<string, AttributeValue>
                {
                    { "tableNumber", new AttributeValue { N = tableNumber.ToString() } }
                }
            ).Wait();
        }

        public Order Select(int tableNumber)
        {
            var response = _client.GetItemAsync(
                tableName: _tableName,
                key: new Dictionary<string, AttributeValue>
                {
                    { "tableNumber", new AttributeValue { N = tableNumber.ToString() } }
                }
            );

            var result = response.Result;
            return ReadOrder(result.Item);
        }

        public Order Select(string requestingUser)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                IndexName = "requestingUser-index",
                KeyConditionExpression = "#ru = :s_ru",
                ExpressionAttributeNames = new Dictionary<String, String> {
                                                {"#ru", "requestingUser"}
                                            },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                                                {":s_ru", new AttributeValue { S = requestingUser }}
                                            },
                ScanIndexForward = true
            };
            
            var resultTask = _client.QueryAsync(request);
            var result = resultTask.Result;

            if (result.Items.Count > 0)
            {
                return ReadOrder(result.Items[0]);
            }
            else
            {
                return null;
            }
        }
        
        public List<Order> GetConfirmedOrders()
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                IndexName = "confirmed-index",
                KeyConditionExpression = "#c = :n_c",
                ExpressionAttributeNames = new Dictionary<String, String> {
                                                {"#c", "confirmed"}
                                            },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                                                {":n_c", new AttributeValue { N = "1" }}
                                            },
                ScanIndexForward = true
            };

            var resultTask = _client.QueryAsync(request);
            var result = resultTask.Result;

            var response = new List<Order>();
            foreach(var item in result.Items)
            {
                response.Add(ReadOrder(item));
            }

            return response;
        }

        public void Update(Order order)
        {
            var brewOrder = GetDocument(order);
            _client.PutItemAsync(
                tableName: _tableName,
                item: GetDocument(order)
            ).Wait();
        }

        private Dictionary<string, AttributeValue> GetDocument(Order order)
        {
            return new Dictionary<string, AttributeValue>
            {
                {"tableNumber", new AttributeValue { N = order.TableNumber.ToString() } },
                {"orderText", new AttributeValue { S = order.OrderText } },
                {"requestingUser", new AttributeValue { S = order.RequestingUser } },
                {"mannersUsed", new AttributeValue {  BOOL = order.MannersUsed } },
                {"confirmed", new AttributeValue { N = order.Confirmed ? "1" : "0" } }
            };
        }

        private Order ReadOrder(Dictionary<string, AttributeValue> document)
        {
            Order order = new Order();

            foreach(var item in document)
            {
                switch (item.Key)
                {
                    case "tableNumber":
                        order.TableNumber = int.Parse(item.Value.N);
                        break;
                    case "orderText":
                        order.OrderText = item.Value.S;
                        break;
                    case "requestingUser":
                        order.RequestingUser = item.Value.S;
                        break;
                    case "mannersUsed":
                        order.MannersUsed = item.Value.BOOL;
                        break;
                    case "confirmed":
                        order.Confirmed = item.Value.N == "1";
                        break;
                }
            }

            return order;
        }
    }
}
