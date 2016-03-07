using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceResponder
{
    public class Responder
    {
        //this class just puts in some functionality 

        private static  Responder _instance;

        public static Responder Instance => _instance ?? (_instance = new Responder());


        public string GetResponse()
        {
            var random = GetRandomNumber();

            switch (random)
            {
                case 1:
                    return "yes, you're right, the answer is 42.";
                case 2:
                    return "sorry, you're wrong, the answer is 42.";
                case 3:
                    return "maybe you're right, maybe not - but the answer is 42.";
            }

            return "sorry, I did not understand the question.";

        }



        private int GetRandomNumber()
        {
            var random = new Random();
            var nextNumber = random.Next(1, 3);

            return nextNumber;
        }



    }
}
