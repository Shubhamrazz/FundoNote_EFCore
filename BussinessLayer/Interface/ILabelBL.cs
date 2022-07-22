﻿using DatabaseLayer.LabelModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLayer.Interface
{
    public interface ILabelBL
    {
        Task AddLabel(int UserId, int NoteId, string LabelName);
        Task<List<LabelModel>> GetAllLabels(int UserId);
    }
}