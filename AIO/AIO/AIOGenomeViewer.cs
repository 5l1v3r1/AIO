﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AIO
{
    public partial class AIOGenomeViewer : Form
    {
        public Genome Genome = null;
        public AIOGenomeViewer()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        const int neuronsize = 20, neuronmid = 10;
        Font font = new Font(FontFamily.GenericMonospace, 8.75f, FontStyle.Regular);
        Pen low = new Pen(Brushes.Red, 2);
        Pen high = new Pen(Brushes.Green, 2);
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            e.Graphics.FillRectangle(Brushes.MidnightBlue, e.ClipRectangle);

            if (Genome == null) return;

            int mid = (int)Math.Round(e.ClipRectangle.Width / 2f);
            int vmid = (int)Math.Round(e.ClipRectangle.Height / 2f);

            List<ViewerNeuron> neurons = new List<ViewerNeuron>();

            int inputx = e.ClipRectangle.Left + (mid - (int)Math.Round((Genome.Input.Length * neuronsize + (Genome.Input.Length - 1f) * neuronsize * 2) / 2f));
            int inputy = e.ClipRectangle.Bottom - neuronsize - 10;
            int curx = inputx;
            for (int i = 0; i < Genome.Input.Length; i++)
            {
                //e.Graphics.FillRectangle(Brushes.White, curx, inputy, neuronsize, neuronsize);
                neurons.Add(new ViewerNeuron() { ID = (int)Genome.Input[i].ID, X = curx, Y = inputy });
                curx += neuronsize * 3;
            }

            int outputx = e.ClipRectangle.Left + (mid - (int)Math.Round((Genome.Output.Length * neuronsize + (Genome.Output.Length - 1f) * neuronsize * 2) / 2f));
            int outputy = 10;
            curx = outputx;
            for (int i = 0; i < Genome.Output.Length; i++)
            {
                //e.Graphics.FillRectangle(Brushes.White, curx, outputy, neuronsize, neuronsize);
                neurons.Add(new ViewerNeuron() { ID = (int)Genome.Output[i].ID, X = curx, Y = outputy });
                curx += neuronsize * 3;
            }
            if (Genome.HiddenLayers.Count > 0)
            {
                int midspace = (int)Math.Round((e.ClipRectangle.Height - 10f * 4f - neuronsize * 2f) / Genome.HiddenLayers.Count);
                int midy = vmid - (int)Math.Round(midspace / 2f);
                //e.Graphics.DrawLine(Pens.CornflowerBlue, mid, midy, mid, midy + midspace);
                int cury = neuronsize + 10 * 2 + midspace * (Genome.HiddenLayers.Count - 1);
                foreach (List<Neuron> layer in Genome.HiddenLayers)
                {
                    int midx = e.ClipRectangle.Left + (mid - (int)Math.Round((layer.Count * neuronsize + (layer.Count - 1f) * neuronsize * 2) / 2f));
                    curx = midx;
                    int tempmidy = (int)Math.Round(cury + midspace / 2f - neuronmid);
                    for (int i = 0; i < layer.Count; i++)
                    {
                        e.Graphics.FillRectangle(Brushes.White, curx, tempmidy, neuronsize, neuronsize);
                        neurons.Add(new ViewerNeuron() { ID = (int)layer[i].ID, X = curx, Y = tempmidy, Threshold = layer[i].Threshold });
                        curx += neuronsize * 3;
                    }
                    cury -= midspace;
                }
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (NeuralConnection c in Genome.Connections)
            {
                ViewerNeuron to = null, from = null;
                foreach (ViewerNeuron vn in neurons)
                {
                    if (to != null && from != null) break;
                    if (vn.ID == c.In.ID) from = vn;
                    else if (vn.ID == c.Out.ID) to = vn;
                }

                if (to == null || from == null) continue;

                e.Graphics.DrawLine((c.Weight == 1f ? Pens.Gray : (c.Weight < 1f ? low : high)), from.X + neuronmid, from.Y, to.X + neuronmid, to.Y + neuronsize);
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            foreach (ViewerNeuron vn in neurons)
            {
                e.Graphics.FillRectangle(Brushes.White, vn.X, vn.Y, neuronsize, neuronsize);
                TextRenderer.DrawText(e.Graphics, vn.Threshold.ToString("0.00"), font, new Point(vn.X, vn.Y), Color.Gray);
            }
        }

        class ViewerNeuron
        {
            public int ID, X, Y;
            public float Threshold;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }
}
