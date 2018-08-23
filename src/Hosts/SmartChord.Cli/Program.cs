using System;
using CommandLine;
using MediatR;
using SmartChord.ChordSheets.Commands;
using SmartChord.ChordSheets.Queries;
using StructureMap;

namespace SmartChord.Cli
{
    class Program
    {
        static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<GetChordsheetOptions>(args)
                .MapResult(
                    GetChordsheet,
                    errs => 1);

            Console.ReadKey();

            return result;
        }

        private static IContainer ConfigureIoc()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssembliesFromApplicationBaseDirectory(assem => assem.FullName.StartsWith("SmartChord."));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });
                cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => ctx.GetInstance);
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => ctx.GetAllInstances);
                cfg.For<IMediator>().Use<Mediator>();
            });

            return container;
        }

        private static int GetChordsheet(GetChordsheetOptions opts)
        {
            var container = ConfigureIoc();

            var mediator = container.GetInstance<IMediator>();

            if (!string.IsNullOrEmpty(opts.Output))
            {
                if (!string.IsNullOrEmpty(opts.Docx))
                {
                    var result = mediator.Send(new CreateWordDocumentFromDocx.Command { SourceFilename = opts.Docx, NewKey = opts.NewKey, DestinationFilename = opts.Output }).Result;
                    Console.Write(result);

                }
                else if (!string.IsNullOrEmpty(opts.Url))
                {
                    var result = mediator.Send(new CreateWordDocumentFromUrl.Command { Url = opts.Url, NewKey = opts.NewKey, DestinationFilename = opts.Output}).Result;
                    Console.Write(result);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(opts.Docx))
                {
                    var result = mediator.Send(new TransposeSheetDocx.Query { FilePath = opts.Docx, NewKey = opts.NewKey }).Result;
                    Console.Write(result);
                }
                else if (!string.IsNullOrEmpty(opts.Url))
                {
                    var result = mediator.Send(new TransposeSheetUrl.Query { Url = opts.Url, NewKey = opts.NewKey }).Result;
                    Console.Write(result);
                }
            }


            return 0;

        }
    }
}
