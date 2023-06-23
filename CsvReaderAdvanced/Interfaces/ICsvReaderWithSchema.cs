using CsvReaderAdvanced;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace CsvReaderAdvanced.Interfaces;

public interface ICsvReaderWithSchema
{
    
    Task<ReaderReport> Import(HttpRequest request);
    Task<ReaderReport> Import(string filePath);
}