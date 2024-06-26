using System.Collections.Generic;

using UnityEngine.UIElements;

public class TabbedMenuController
{

    private const string tabClassName = "Tab";
    private const string currentlySelectedTabClassName = "CurrentlySelectedTab";
    private const string unselectedTabClassName = "UnselectedContent";

    private const string tabNameSuffix = "Tab";
    private const string contentNameSuffix = "Content";


    private readonly VisualElement root;



    public TabbedMenuController(VisualElement root)
    {
        this.root = root;
    }

    public void RegisterTabCallback()
    {
        UQueryBuilder<Label> tabs = GetAllTabs();
        tabs.ForEach((Label tab) =>
        {
            tab.RegisterCallback<ClickEvent>(TabOnClick);
        });
    }


    private void TabOnClick(ClickEvent clickEvent)
    {
        Label clickedTab = clickEvent.currentTarget as Label;
        if (!IsTabCurrentlySelected(clickedTab))
        {
            List<Label> List_LabelsForTabs = GetAllTabs().ToList();
            for (int i = 0; i < List_LabelsForTabs.Count; i++)
            {
                if (List_LabelsForTabs[i] != clickedTab && IsTabCurrentlySelected(List_LabelsForTabs[i]))
                {
                    UnselectTab(List_LabelsForTabs[i]);
                }
            }

            SelectTab(clickedTab);
        }
    }

    private static bool IsTabCurrentlySelected(Label tab)
    {
        return tab.ClassListContains(currentlySelectedTabClassName);
    }

    private UQueryBuilder<Label> GetAllTabs()
    {
        return root.Query<Label>(className: tabClassName);
    }

    private void SelectTab(Label tab)
    {
        tab.AddToClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.RemoveFromClassList(unselectedTabClassName);
    }

    private void UnselectTab(Label tab)
    {
        tab.RemoveFromClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.AddToClassList(unselectedTabClassName);
    }

    private static string GenerateContentName(Label tab)
    {
        return tab.name.Replace(tabNameSuffix, contentNameSuffix);
    }

    private VisualElement FindContent(Label tab)
    {
        return root.Q(GenerateContentName(tab));
    }

}
