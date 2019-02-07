using System;
using Analyzer.Domain.Stats;
using TddBuddy.CleanArchitecture.Domain.Messages;

namespace Analyzer.Presenter
{
    public class JsonPresenter : IPresenter
    {
        public void Respond(ErrorOutputMessage output)
        {
            throw new NotImplementedException();
        }

        public void Respond(StatsOuput output)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}