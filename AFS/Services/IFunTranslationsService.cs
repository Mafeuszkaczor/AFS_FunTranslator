using System;
namespace AFS.Services
{
	public interface IFunTranslationsService
	{
        Task<string> TranslateAsync(string text, string translationType);
    }
}

