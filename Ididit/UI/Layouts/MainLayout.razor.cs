﻿using Ididit.App;
using Ididit.Data;
using Ididit.Data.Models;
using Ididit.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Layouts;

public partial class MainLayout
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [Inject]
    DirectoryBackup _directoryBackup { get; set; } = null!;

    [Inject]
    JsonBackup _jsonBackup { get; set; } = null!;

    [Inject]
    YamlBackup _yamlBackup { get; set; } = null!;

    [Inject]
    MarkdownBackup _markdownBackup { get; set; } = null!;

    [Inject]
    JsInterop _jsInterop { get; set; } = null!;


    // calling _repository.Initialize() from OnInitializedAsync() or OnParametersSetAsync() doesn't work

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        // This is called 2 times. If isInitialized is used to return after first call, then it doesn't work.
        await _repository.Initialize();

        await base.SetParametersAsync(ParameterView.Empty);
    }

    async Task Import(InputFileChangeEventArgs e)
    {
        Stream stream = e.File.OpenReadStream(maxAllowedSize: 5242880);

        if (e.File.Name.EndsWith(".json"))
        {
            DataModel data = await _jsonBackup.ImportData(stream);

            await _repository.AddData(data);
        }

        if (e.File.Name.EndsWith(".yaml"))
        {
            DataModel data = await _yamlBackup.ImportData(stream);

            await _repository.AddData(data);
        }
    }

    async Task ExportJson()
    {
        await _jsonBackup.ExportData(_repository);
    }

    async Task ExportYaml()
    {
        await _yamlBackup.ExportData(_repository);
    }

    async Task ImportMarkdown(InputFileChangeEventArgs e)
    {
        // TODO: use the real selectedCategory

        CategoryModel? selectedCategory = _repository.CategoryList.FirstOrDefault();

        if (selectedCategory != null)
        {
            IEnumerable<IBrowserFile> browserFiles = e.GetMultipleFiles(e.FileCount).Where(browserFile => browserFile.Name.EndsWith(".md"));

            foreach (IBrowserFile browserFile in browserFiles)
            {
                string name = Path.GetFileNameWithoutExtension(browserFile.Name);

                Stream stream = browserFile.OpenReadStream();

                await _markdownBackup.ImportData(selectedCategory, stream, name);
            }
        }
    }

    async Task ExportMarkdown()
    {
        await _markdownBackup.ExportData(_repository);
    }

    async Task ImportDirectory()
    {
        NodeContent? directory = await _jsInterop.ReadDirectoryFiles();

        if (directory != null)
            _directoryBackup.ImportData(directory);
    }

    async Task ExportDirectory()
    {
        NodeContent[] nodes = _directoryBackup.ExportData(_repository);

        await _jsInterop.WriteDirectoryFiles(nodes);
    }
}
