using System;
using System.Collections.Generic;
using System.Text;
using Analyzer.Domain.Stats;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Output;

namespace Analyzer.Presenter
{
    public interface IPresenter : IRespondWithSuccessOrError<StatsOuput, ErrorOutputMessage>
    {
        void Render();
    }
}
