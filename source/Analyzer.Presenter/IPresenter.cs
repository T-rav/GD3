using Analyzer.Domain.SourceControl;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Output;

namespace Analyzer.Presenter
{
    public interface IPresenter : IRespondWithSuccessOrError<CodeAnalysis, ErrorOutputMessage>
    {
        void Render();
    }
}
