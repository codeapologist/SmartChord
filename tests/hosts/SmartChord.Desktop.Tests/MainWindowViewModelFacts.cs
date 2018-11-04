using MediatR;
using NSubstitute;
using SmartChord.ChordSheets.Commands;
using Thinktecture;
using Xunit;

namespace SmartChord.Desktop.Tests
{
    public class MainWindowViewModelFacts
    {
        [Fact]
        public void When_source_is_a_docx_then_docx_command_is_called()
        {
            var mediator = Substitute.For<IMediator>();
            var environment = Substitute.For<IEnvironment>();
            var dialogService = Substitute.For<IDialogService>();
            var viewModel = new MainWindowViewModel(mediator, environment, dialogService)
            {
                NewKey = "A",
                Source = @"C:\test\song.docx",
                Destination = @"C:\test\song_A.docx"
            };

            viewModel.OnGo();

            mediator.Received().Send(Arg.Is<CreateWordDocumentFromDocx.Command>(x => x.NewKey == "A" &&
            x.SourceFilename == @"C:\test\song.docx" &&
            x.DestinationFilename == @"C:\test\song_A.docx"));
        }
    }
}
