using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Blongo.Data.hmmmm;

namespace Blongo.Models.dbConnectivity
{
    class dbConnectivity
    {
        private readonly IMongoDatabase _mongoDatabase;

        public dbConnectivity()
        {
            var client = new MongoClient("mongodb://localhost:27017");

            _mongoDatabase = client.GetDatabase("userData");

        }

        public string? getUserId(string user_name)
        {

            var data = _mongoDatabase.GetCollection<BsonDocument>("userRecords");

            string? result = "";


            var filter = Builders<BsonDocument>.Filter.Eq("user_name", user_name);

            var userExists = data.Find(filter).FirstOrDefault();

            result = userExists.GetElement("_id").Value.ToString();



            return (result);



        }


        public void saltify(string? uid, byte[] salt)
        {

            var data = _mongoDatabase.GetCollection<BsonDocument>("saltyRecords");

            var document = new BsonDocument { { "_id", uid }, { "salt", Convert.ToBase64String(salt) } };

            data.InsertOne(document);
            Console.WriteLine("Successfully Created");

        }


        public string signUp(string fname, string lname, string gender, string user_name, DateTime dob, int? age, string nyn)
        {

            var data = _mongoDatabase.GetCollection<BsonDocument>("userRecords");

            string result = "user already exists";
            user_name = user_name.ToLower();


            age = 12 * (DateTime.Today.Year - dob.Year) + (DateTime.Today.Month - dob.Month);
            age /= 12;

            byte[] salt;
            string superNyn;

            Cage c = new Cage();
            c.caging(nyn, out superNyn, out salt);



            try
            {
                var document = new BsonDocument { { "user_name", user_name }, { "fname", fname }, { "lname", lname }, { "gender", gender }, { "age", age }, { "nyn", superNyn } };
                data.InsertOne(document);
                result = "Successfully Created";
                Console.WriteLine("yeye");

            }


            catch (MongoException m)
            {


                if (m.GetHashCode() == 21454193)
                {
                    result = "Duplicate UserId";
                }

            }


            string? uid = getUserId(user_name);
            saltify(uid, salt);
            return result;





        }




        public string login(string name, string nyn)
        {

            var data = _mongoDatabase.GetCollection<BsonDocument>("userRecords");


            string result = "";

            var filter = Builders<BsonDocument>.Filter.Eq("name", name);

            var userExists = data.Find(filter).FirstOrDefault();


            if (userExists == null)
            {
                result = "user not found";
                return result;
            }
            else
            {
                var document = new BsonDocument { { "name", name }, { "nyn", nyn } };
                //var passFilter = Builders<BsonDocument>.Filter. );

                var passCheck = data.Find(document).FirstOrDefault();
                if (passCheck == null)
                {
                    result = "Incorrect Password BOZO";
                    return result;
                }
                else
                    result = "SUCCCceEESSSSSS";

            }

            return result;
        }
    }
}



/* class Program
 {
     static void Main(string[] args)
     {
         dbconnectivity db = new dbconnectivity();

         //  string insertResult = db.insertNewUser("Obama1", 5, "yong");
         string loginResult = db.login("Obama8", "yeah");
         Console.WriteLine(loginResult);
     }
 }

} */