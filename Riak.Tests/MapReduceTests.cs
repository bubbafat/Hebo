using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riak.Client;

namespace Riak.Tests
{
    [TestClass]
    public class MapReduceTests
    {
        /*
 * 
 *  
 {
    "inputs": "Ratings",
    "query": [
        {
            "map":{
                "keep": false,
                "language": "javascript",
                "source": "function(value, keyData, arg) { 
            var data = Riak.mapValuesJson(value);
            if(data[0].stars > 3) {
                return [data[0]];
            }

            return [];
        }"
            } 
        },
        {
            "reduce": {
                "keep": true,
                "language": "javascript",
        "source": "function(v) {
            return [v]
        }" 
            } 
        } 
    ]
}
 * 
 * */

        [TestMethod]
        public void MRTest1()
        {
            Assert.Inconclusive("This test is not ready yet.");

            RiakClient c = new RiakClient(Settings.RiakServerUri);
            Bucket b = c.Bucket("Missing");

            // function(value, keyData, arg) { var data = Riak.mapValuesJson(value); return [data[0]]; }
            MapReduce mr1 = new MapReduce()
                .Input(b)
                .Map(new Phase(
                         new JsonFunction(
                             "function(value, keyData, arg) { var data = Riak.mapValuesJson(value); return [data[0]]; }")))
                .Reduce(new Phase(
                            new JsonFunction("function(value) { return [value]; }"))
                            {
                                Keep = true
                            });

            RiakHttpResponse response = mr1.Run();

        }
    }
}
