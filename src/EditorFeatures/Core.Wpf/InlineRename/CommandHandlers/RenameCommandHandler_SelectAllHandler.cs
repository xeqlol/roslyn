﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.UI.Commanding;
using Microsoft.VisualStudio.Text.UI.Commanding.Commands;

namespace Microsoft.CodeAnalysis.Editor.Implementation.InlineRename
{
    internal partial class RenameCommandHandler :
        ILegacyCommandHandler<SelectAllCommandArgs>
    {
        public CommandState GetCommandState(SelectAllCommandArgs args, Func<CommandState> nextHandler)
        {
            return GetCommandState(nextHandler);
        }

        public void ExecuteCommand(SelectAllCommandArgs args, Action nextHandler)
        {
            if (ExecuteSelectAll(args.SubjectBuffer, args.TextView))
            {
                return;
            }

            nextHandler();
        }

        private bool ExecuteSelectAll(ITextBuffer subjectBuffer, ITextView view)
        {
            if (_renameService.ActiveSession == null)
            {
                return false;
            }

            var caretPoint = view.GetCaretPoint(subjectBuffer);
            if (caretPoint.HasValue)
            {
                if (_renameService.ActiveSession.TryGetContainingEditableSpan(caretPoint.Value, out var span))
                {
                    if (view.Selection.Start.Position != span.Start.Position ||
                        view.Selection.End.Position != span.End.Position)
                    {
                        view.SetSelection(span);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
