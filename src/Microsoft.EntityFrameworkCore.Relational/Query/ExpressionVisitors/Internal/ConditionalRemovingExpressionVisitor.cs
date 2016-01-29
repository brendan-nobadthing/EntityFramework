// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    public class ConditionalRemovingExpressionVisitor : ExpressionVisitorBase
    {
        public override Expression Visit([CanBeNull] Expression node)
        {
            var selectExpression = node as SelectExpression;

            var conditionalExpression = selectExpression?.Projection.FirstOrDefault() as ConditionalExpression;

            if (selectExpression?.Projection.Count == 1
                && conditionalExpression != null
                && conditionalExpression.Type == typeof(bool)
                && conditionalExpression.IfTrue.NodeType == ExpressionType.Constant
                && conditionalExpression.IfFalse.NodeType == ExpressionType.Constant
                && (bool)(conditionalExpression.IfTrue as ConstantExpression).Value
                && !(bool)(conditionalExpression.IfFalse as ConstantExpression).Value)
            {
                return Visit(conditionalExpression.Test);
            }

            return base.Visit(node);
        }
    }
}
