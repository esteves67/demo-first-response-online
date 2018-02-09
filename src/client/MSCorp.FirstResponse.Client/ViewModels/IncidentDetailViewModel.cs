﻿using MSCorp.FirstResponse.Client.Models;
using MSCorp.FirstResponse.Client.ViewModels.Base;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class IncidentDetailViewModel : ViewModelBase
    {
        private IncidentModel _incident;

        public IncidentModel Incident
        {
            get { return _incident; }
            set
            {
                if (_incident != value)
                {
                    _incident = value;
                    RaisePropertyChanged(() => Incident);
                    RaisePropertyChanged(() => Suspects);
                }
            }
        }

        public List<SuspectModel> Suspects
        {
            get { return _incident?.Identities; }
            set
            {
                _incident.Identities = value;
                RaisePropertyChanged(() => Suspects);
            }
        }

        public ICommand ForceNavigationCommand => new Command(() => MessagingCenter.Send(_incident, MessengerKeys.NavigateToCurrentIncident));

        public ICommand IdentifySuspectCommand => new Command(IdentifySuspect);

        public ICommand NewTicketCommand => new Command<SuspectModel>(NewTicket);

        public ICommand NewEpcrCommand => new Command<SuspectModel>(NewEpcr);

        public ICommand CloseIncidentCommand => new Command(CloseIncident);        

        public IncidentDetailViewModel()
        {
            MessagingCenter.Subscribe<IncidentListViewModel>(this, MessengerKeys.SelectedIncidentChanged, UpdateCurrentIncidentFromMessage);
            MessagingCenter.Subscribe<List<SuspectModel>>(this, MessengerKeys.SelectedSuspectsChanged, UpdateSuspectsInIncidentModel);
        }

        private void UpdateCurrentIncidentFromMessage(IncidentListViewModel list)
        {
            Incident = list.SelectedIncident;
        }

        private void UpdateSuspectsInIncidentModel(List<SuspectModel> suspects)
        {
            Suspects = suspects;
        }        
        
        private void CloseIncident()
        {
            MessagingCenter.Send<ViewModelBase>(this, MessengerKeys.CloseIncident);
        }

        private async void IdentifySuspect()
        {
            await NavigationService.NavigateToPopupAsync<SuspectViewModel>(true);
        }

        private async void NewTicket(SuspectModel suspect)
        {
            var navParameter = new IncidentInvolvedNavigationParameter(suspect, Incident);
            await NavigationService.NavigateToPopupAsync<NewTicketViewModel>(navParameter, true);
        }

        private async void NewEpcr(SuspectModel person)
        {
            var navParameter = new IncidentInvolvedNavigationParameter(person, Incident);
            await NavigationService.NavigateToPopupAsync<NewEpcrViewModel>(navParameter, true);
        }
    }
}
