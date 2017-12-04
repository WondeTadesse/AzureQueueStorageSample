//|---------------------------------------------------------------|
//|                         AZURE QUEUE STORAGE                   |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2017 - Present         |
//|---------------------------------------------------------------|
//|                         AZURE QUEUE STORAGE                   |
//|---------------------------------------------------------------|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureQueueStorageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            new AzureQueueProcessor().ProcessAzureQueues().Wait();
            Console.ReadKey();
        }
    }
}
