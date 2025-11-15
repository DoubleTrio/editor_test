using System;
using System.Linq;
using System.Text.RegularExpressions;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Utility;

public static class NodeHelper
{
    public static bool FilterRecursive(NodeBase node, string filter)
    {
        bool match = string.IsNullOrWhiteSpace(filter);

        var tokens = Regex.Split(node.Title, @"[\s_:]+");

        match |= tokens.Any(token =>
            token.StartsWith(filter, StringComparison.OrdinalIgnoreCase));

        bool childMatch = false;
        foreach (var child in node.SubNodes)
        {
            if (FilterRecursive(child, filter))
                childMatch = true;
        }

        node.IsVisible = match || childMatch;

        if (!string.IsNullOrWhiteSpace(filter))
        {
            node.IsExpanded = match || childMatch;

            if (match && node.SubNodes.Any())
            {
                foreach (var child in node.SubNodes)
                    child.IsVisible = true;
            }
        }

        return node.IsVisible;
    }
}
