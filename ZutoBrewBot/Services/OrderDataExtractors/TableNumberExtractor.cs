using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZutoBrewBot.Models.Responses;
using ZutoBrewBot.Services.OrderDataExtractors.Interfaces;

namespace ZutoBrewBot.Services.OrderDataExtractors
{
    public class TableNumberExtractor : ITableNumberExtractor
    {
        private readonly string _table = "table";

        public OrderTextTableExtractorResponse GetTableNumber(string orderString)
        {
            var response = new OrderTextTableExtractorResponse
            {
                TableFound = false,
                TableNumber = 0,
                TableText = orderString
            };

            // check that orderString contains the text "table" at some point
            if (orderString == null || !orderString.ToLower().Contains(_table))
            {
                return response;
            }

            // break the request down into an array of words, split by white space
            var wordsArray = orderString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            // find the first element with "table" in it
            int tableIndex = FindTableIndex(wordsArray);

            // get the table number from the text immediately following table
            string toSearch = string.Join(" ", wordsArray[tableIndex].Substring(wordsArray[tableIndex].ToLower().IndexOf(_table)),
                                                tableIndex < (wordsArray.Length - 1) ? wordsArray[tableIndex + 1] : string.Empty);
            response.TableText = toSearch.Trim();
            int tableNumber = FindNumberInString(toSearch);

            if (tableNumber > 0)
            {
                response.TableFound = true;
                response.TableNumber = tableNumber;
            }

            return response;
        }

        private int FindTableIndex(string[] wordsArray)
        {
            int tableIndex = 0, i = 0;
            while (i < wordsArray.Length && tableIndex == 0)
            {
                if (wordsArray[i].ToLower().Contains(_table))
                {
                    tableIndex = i;
                }

                i++;
            }
            return tableIndex;
        }

        private int FindNumberInString(string toSearch)
        {
            StringBuilder foundNumberString = new StringBuilder();

            bool insideNumber = false, numberFinished = false;
            var searchArray = toSearch.ToCharArray();
            int i = 0;

            while (i < searchArray.Length && !(insideNumber && numberFinished))
            {
                Char c = searchArray[i];
                if (Char.IsNumber(c))
                {
                    foundNumberString.Append(c);
                    insideNumber = true;
                }
                else
                {
                    if (insideNumber)
                    {
                        numberFinished = true;
                    }
                }

                i++;
            }
            
            // If there aren't any numbers after "table", we should just return 0 and let the client handle it
            if (foundNumberString.Length == 0)
            {
                return 0;
            }

            if (int.TryParse(foundNumberString.ToString(), out int foundNumber))
            {
                return foundNumber;
            }
            else
            {
                throw new Exception("Found a number that isn't a number");
            }
        }
    }
}
