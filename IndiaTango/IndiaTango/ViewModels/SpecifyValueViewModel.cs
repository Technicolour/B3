﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using IndiaTango.Models;

namespace IndiaTango.ViewModels
{
    class SpecifyValueViewModel : BaseViewModel
    {
        private readonly SimpleContainer _container;
        private readonly IWindowManager _windowManager;

        private bool _okPressed = false;
        

        public SpecifyValueViewModel(IWindowManager windowManager, SimpleContainer container)
        {
            _container = container;
            _windowManager = windowManager;
        }

        public string Title { get { return "Specify value"; } }

        private string _text = null;
        public string Text { get { return _text; } set { _text = value; NotifyOfPropertyChange(()=>Text); } }


        public void btnOK()
        {
            _okPressed = true;
            this.TryClose();
        }

        public void Closing()
        {
            if (_okPressed == false)
                Text = null;
        }
    }
}