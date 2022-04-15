﻿using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public sealed partial class EditCategoryComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [Inject]
    Theme Theme { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public CategoryModel? ParentCategory { get; set; } = null!;

    [Parameter]
    public CategoryModel? Category { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> CategoryChanged { get; set; }

    public bool editName;

    void EditName()
    {
        if (Category != null)
            editName = true;
    }

    void CancelEdit()
    {
        editName = false;
    }

    async Task SaveName()
    {
        editName = false;

        if (Category != null)
            await _repository.UpdateCategoryName(Category.Id, Category.Name);

        await CategoryChanged.InvokeAsync(Category);
    }

    async Task NewCategory()
    {
        editName = true;

        CategoryModel category = ParentCategory != null ? ParentCategory.CreateCategory() : _repository.CreateCategory();

        await _repository.AddCategory(category);

        Category = category;
        await CategoryChanged.InvokeAsync(Category);
    }

    async Task DeleteCategory()
    {
        if (Category == null)
            return;

        if (ParentCategory != null)
            ParentCategory.CategoryList.Remove(Category);

        await _repository.DeleteCategory(Category.Id);

        Category = null;
        await CategoryChanged.InvokeAsync(Category);
    }
}
