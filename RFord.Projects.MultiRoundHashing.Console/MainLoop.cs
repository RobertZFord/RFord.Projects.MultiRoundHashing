using Microsoft.Extensions.Hosting;
using RFord.Projects.MultiRoundHashing.Core.Services;
using RFord.Projects.MultiRoundHashing.Core;
using Spectre.Console;
using System.Security.Cryptography;

namespace RFord.Projects.MultiRoundHashing.Console
{
    class MainLoop : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private readonly IMultiRoundHasher _multiRoundHasher;
        private readonly IDataProvider _dataProvider;
        private readonly IFileSystemAccess _fileSystemAccess;

        public MainLoop(
            IHostApplicationLifetime hostApplicationLifetime,
            IMultiRoundHasher multiRoundHasher,
            IDataProvider dataProvider,
            IFileSystemAccess fileSystemAccess
        )
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _multiRoundHasher = multiRoundHasher;
            _dataProvider = dataProvider;
            _fileSystemAccess = fileSystemAccess;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // select the source to hash, text or file

            IPrompt<HashSource> hashSourceTypePrompt = new TextPrompt<HashSource>("What is the hash source type?", StringComparer.OrdinalIgnoreCase)
                .AddChoices(Enum.GetValues<HashSource>())
                ;

            HashSource selectedSourceType = AnsiConsole.Prompt(hashSourceTypePrompt);



            // specify the path or string that will be hashed

            IPrompt<string> hashSourcePrompt = new TextPrompt<string>(selectedSourceType == HashSource.File ? "Specify a path:" : "Specify a UTF-16 string:")
                .Validate(x => {
                    if (selectedSourceType == HashSource.File)
                    {
                        if (!_fileSystemAccess.TryCleanFilePath(x, out string cleanedFilePath))
                        {
                            return ValidationResult.Error("Invalid file path!");
                        }
                        if (!_fileSystemAccess.FileExists(cleanedFilePath))
                        {
                            return ValidationResult.Error("Unable to locate the specified file");
                        }
                    }
                    return ValidationResult.Success();
                })
                ;

            if (!_fileSystemAccess.TryCleanFilePath(filePath: AnsiConsole.Prompt(hashSourcePrompt), out string hashSource))
            {
                throw new InvalidOperationException();
            }



            // specify the hash algorithm to use

            IPrompt<SupportedHashAlgorithm> hashAlgorithmPrompt = new TextPrompt<SupportedHashAlgorithm>("Which hash algorithm?", StringComparer.OrdinalIgnoreCase)
                .AddChoices(Enum.GetValues<SupportedHashAlgorithm>())
                .WithConverter(hash => hash.ToString().ToUpperInvariant())
                ;

            SupportedHashAlgorithm selectedHashAlgorithm = AnsiConsole.Prompt(hashAlgorithmPrompt);



            // specify how many rounds of the hash to perform

            IPrompt<int> roundsPrompt = new TextPrompt<int>("How many rounds?")
                .Validate(i => i > 0 && i <= 100_000_000, "Please specify a value between 1 and 10,000,000")
                ;

            int roundCount = AnsiConsole.Prompt(roundsPrompt);



            // put all the things together

            string selectedHashAlgorithmUppercase = selectedHashAlgorithm.ToString().ToUpperInvariant();

            FormattableString pendingActionDisplay = $"Performing [blue]{roundCount}[/] rounds of [blue]{selectedHashAlgorithmUppercase}[/] on [blue]{(selectedSourceType == HashSource.File ? hashSource : "provided string")}[/]";

            AnsiConsole.MarkupLineInterpolated(pendingActionDisplay);

            byte[] result = new byte[0];
            using (Stream input = _dataProvider.GetStream(sourceType: selectedSourceType, sourceData: hashSource))
            {
                HashAlgorithm hash = selectedHashAlgorithm switch
                {
                    SupportedHashAlgorithm.Sha1 => SHA1.Create(),
                    SupportedHashAlgorithm.Sha256 => SHA256.Create(),
                    SupportedHashAlgorithm.Sha384 => SHA384.Create(),
                    SupportedHashAlgorithm.Sha512 => SHA512.Create(),
                    _ => throw new InvalidOperationException()
                };

                AnsiConsole
                    .Progress()
                    .Columns(
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn(),
                        new RemainingTimeColumn(),
                        new SpinnerColumn()
                    )
                    .AutoClear(true)
                    .Start(ctx => {
                        ProgressTask progress = ctx.AddTask("Performing hash", true, roundCount);


                        // Q: ...can we maybe inject a callback function here?
                        // and while the hash rounds are occuring, each iteration calls the callback?
                        // A: yes, yes we can.
                        result = _multiRoundHasher.ComputeHash(
                            input,
                            hash,
                            roundCount,
                            () =>
                            {
                                progress.Increment(1.0);
                            }
                        );


                    })
                    ;
            }

            FormattableString output = $"[blue]{selectedHashAlgorithmUppercase}[/] result after [blue]{roundCount}[/] rounds: {string.Join("", result.Select(x => x.ToString("X2")))}";

            AnsiConsole.MarkupLineInterpolated(output);



            _hostApplicationLifetime.StopApplication();

            return Task.CompletedTask;
        }
    }

}