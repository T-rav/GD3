using System;
using System.IO;
using Analyzer.Domain.SourceControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TddBuddy.CleanArchitecture.Domain.Messages;

namespace Analyzer.Presenter
{
    public class JsonPresenter : IPresenter
    {
        private CodeAnalysis _output;
        private ErrorOutputMessage _errors;

        public void Respond(ErrorOutputMessage output)
        {
            _errors = output;
        }

        public void Respond(CodeAnalysis output)
        {
            _output = output;
        }

        public void Render()
        {
            if (_errors != null)
            {
                var msg = string.Join(",", _errors.Errors);
                Console.WriteLine($"the following errors occured: {msg}");
            }

            // client\src\assets
            // C:\Systems\GD3\source\Analyzer\bin\Debug\netcoreapp2.1\Analyzer.Presenter.dll
            //var location = Assembly.GetExecutingAssembly().Location;
            var location = "C:\\Systems\\GD3\\client\\src\\assets\\test-data.json";
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var jsonData = JsonConvert.SerializeObject(_output,new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(location, jsonData);
            Console.WriteLine("Now open the client project and run");
        }
    }
}