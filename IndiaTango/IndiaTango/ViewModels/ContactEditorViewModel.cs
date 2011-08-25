﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Caliburn.Micro;
using IndiaTango.Models;

namespace IndiaTango.ViewModels
{
    class ContactEditorViewModel : BaseViewModel
    {
        private readonly IWindowManager _windowManager = null;
        private readonly SimpleContainer _container = null;
    	private Contact _contact;
        private ObservableCollection<Contact> _allContacts = new ObservableCollection<Contact>();

        public ContactEditorViewModel(IWindowManager manager, SimpleContainer container)
        {
            _windowManager = manager;
            _container = container;
        }

        public string Title
        {
            get { return _contact == null ? "Create New Contact" : "Edit Contact"; }
        }

        public ObservableCollection<Contact> AllContacts
        {
            get { return _allContacts; }
            set { _allContacts = value; }
        }

		public string ContactFirstName { get; set; }

		public string ContactLastName { get; set; }

		public string ContactEmail { get; set; }

		public string ContactPhone { get; set; }

		public string ContactBusiness { get; set; }

		public Contact Contact
		{
			get { return _contact; }
			set
			{
				_contact = value;

				if (_contact != null)
				{
					ContactFirstName = _contact.FirstName;
					ContactLastName = _contact.LastName;
					ContactEmail = _contact.Email;
					ContactPhone = _contact.Phone;
					ContactBusiness = _contact.Business;
				}
				else
				{
					ContactFirstName = "";
					ContactLastName = "";
					ContactEmail = "";
					ContactPhone = "";
					ContactBusiness = "";
				}

				NotifyOfPropertyChange(() => ContactFirstName);
				NotifyOfPropertyChange(() => ContactLastName);
				NotifyOfPropertyChange(() => ContactEmail);
				NotifyOfPropertyChange(() => ContactPhone);
				NotifyOfPropertyChange(() => ContactBusiness);
			}
		}

        public void btnCancel()
        {
            this.TryClose();
        }

        public void btnSave()
        {
            if(Contact == null)
            {
                // New contact!
                try
                {
                    Contact c = new Contact(ContactFirstName, ContactLastName, ContactEmail, ContactBusiness, ContactPhone);

                    AllContacts.Add(c);

                    Contact.ExportAll(AllContacts);

                    this.TryClose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Update the contact
                try
                {
                    // TODO: make this pretty!
                    AllContacts.Remove(Contact);

                    Contact.FirstName = ContactFirstName;
                    Contact.LastName = ContactLastName;
                    Contact.Email = ContactEmail;
                    Contact.Business = ContactBusiness;
                    Contact.Phone = ContactPhone;

                    AllContacts.Add(Contact);

                    // Updating the object itself, so just re-serialise
                    Contact.ExportAll(AllContacts);

                    this.TryClose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
