﻿using Fotografix.Win2D.Composition.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition
{
    internal sealed class AdjustmentLayerNode : LayerNode
    {
        private readonly AdjustmentLayer layer;
        private readonly CrossFadeEffect crossFadeEffect;
        private AdjustmentNode adjustmentNode;

        public AdjustmentLayerNode(AdjustmentLayer layer) : base(layer)
        {
            this.layer = layer;
            this.crossFadeEffect = new CrossFadeEffect();
            RegisterAdjustment();
            UpdateOutput();
        }

        public override void Dispose()
        {
            UnregisterAdjustment();
            crossFadeEffect.Dispose();
            base.Dispose();
        }

        protected override void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AdjustmentLayer.Adjustment))
            {
                UnregisterAdjustment();
                RegisterAdjustment();
            }

            base.OnLayerPropertyChanged(sender, e);
        }

        protected override ICanvasImage ResolveOutput(ICanvasImage background)
        {
            if (!layer.Visible || layer.Opacity == 0 || background == null)
            {
                return background;
            }

            ICanvasImage blendOutput = BlendAdjustment(background);

            if (layer.Opacity == 1)
            {
                return blendOutput;
            }

            crossFadeEffect.Source1 = background;
            crossFadeEffect.Source2 = blendOutput;
            crossFadeEffect.CrossFade = layer.Opacity;
            return crossFadeEffect;
        }

        private ICanvasImage BlendAdjustment(ICanvasImage background)
        {
            adjustmentNode.Input = background;

            if (layer.BlendMode == BlendMode.Normal)
            {
                return adjustmentNode.Output;
            }

            return Blend(adjustmentNode.Output, background);
        }

        private void RegisterAdjustment()
        {
            this.adjustmentNode = NodeFactory.Adjustment.Create(layer.Adjustment);
            adjustmentNode.Invalidated += OnAdjustmentInvalidated;
            adjustmentNode.OutputChanged += OnAdjustmentOutputChanged;
        }

        private void UnregisterAdjustment()
        {
            adjustmentNode.OutputChanged -= OnAdjustmentOutputChanged;
            adjustmentNode.Invalidated -= OnAdjustmentInvalidated;
            adjustmentNode.Dispose();
        }

        private void OnAdjustmentInvalidated(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnAdjustmentOutputChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }
    }
}