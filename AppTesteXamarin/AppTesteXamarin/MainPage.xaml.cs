using AppTesteXamarin.Enum;
using AppTesteXamarin.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTesteXamarin
{
    public partial class MainPage : ContentPage
    {
        public List<Candidate> candidateList  { get; set; }
        public List<Experience> experienceList { get; set; }

        public MainPage()
        {
            InitializeComponent();
            CarregaCandidateListView().Wait();
            CarregaExperiencePicker().Wait();
        }

        public async Task CarregaCandidateListView()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://app.ifs.aero/EternalBlue/api/candidates/").ConfigureAwait(false))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        List<Candidate> reservationList = new List<Candidate>();
                        candidateList = JsonConvert.DeserializeObject<List<Candidate>>(apiResponse);
                        this.lstCandidates.ItemsSource = candidateList;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        public async Task CarregaExperiencePicker()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://app.ifs.aero/EternalBlue/api/technologies/").ConfigureAwait(false))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        List<Experience> reservationList = new List<Experience>();
                        experienceList = JsonConvert.DeserializeObject<List<Experience>>(apiResponse);
                        this.experienceEntry.ItemsSource = experienceList;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        void OnButtonClicked(object sender, EventArgs args)
        {
            if ((Experience)experienceEntry.SelectedItem != null)
            {
                FilterCandidateList();
            } else
            {
                DisplayAlert("Alert", $"Please, select one experience.", "OK");
            }
        }

          void OnShowApproved(object sender, EventArgs args)
        {
            this.lstCandidates.ItemsSource = candidateList.Where(c => c.status == EStatus.Approved).ToList();
        }

        void OnSwiped(object sender, SwipedEventArgs e)
        {
            foreach (Candidate obj in candidateList)
            {
                if (obj.candidateId == Guid.Parse(e.Parameter.ToString()))
                {
                    if (obj.status == EStatus.Undefined)
                    {
                        switch (e.Direction)
                        {
                            case SwipeDirection.Left:
                                obj.status = EStatus.Disapproved;
                                DisplayAlert("Alert", $"Candidate { obj.fullName } { EStatus.Disapproved.ToString() }", "OK");
                                break;
                            case SwipeDirection.Right:
                                obj.status = EStatus.Approved;
                                DisplayAlert("Alert", $"Candidate { obj.fullName } { EStatus.Approved.ToString() }", "OK");
                                break;
                        }
                        FilterCandidateList();
                    }
                    else
                    {
                        DisplayAlert("Alert", $"Candidate already { EStatus.Approved.ToString() }", "OK");
                    }
                }
            }
        }
        void FilterCandidateList()
        {
            if ((Experience)experienceEntry.SelectedItem == null)
            {
                this.lstCandidates.ItemsSource = candidateList.Where(c => c.status == EStatus.Undefined).ToList();
            } else
            {
                Guid technologyId = ((Experience)experienceEntry.SelectedItem).guid;
                int yearsOfExperience = int.Parse(yearsOfExperienceEntry.Text);

                this.lstCandidates.ItemsSource = candidateList.Where(c => c.status == EStatus.Undefined &&
                                                                     c.experience.Any(exp => exp.technologyId == technologyId && exp.yearsOfExperience == yearsOfExperience)).ToList();
            }
        }
        private void OnYearsOfExperienceEntryChanged(object sender, TextChangedEventArgs e)
        {
            //lets the Entry be empty
            if (string.IsNullOrEmpty(e.NewTextValue)) return;

            if (!double.TryParse(e.NewTextValue, out double value))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }
    }
}
