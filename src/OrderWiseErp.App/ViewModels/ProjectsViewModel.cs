using System.Collections.ObjectModel;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class ProjectsViewModel : ObservableObject
{
    private readonly IAppDataService _dataService;
    private Project? _selectedProject;
    private string _projectNo = string.Empty;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private DateTime? _startDate = DateTime.Today;
    private DateTime? _endDate;
    private string _lastAction = "Ready";

    public ProjectsViewModel(IAppDataService dataService)
    {
        _dataService = dataService;

        AddOrUpdateCommand = new RelayCommand(AddOrUpdate);
        ClearFormCommand = new RelayCommand(ClearForm);
        DeleteCommand = new RelayCommand(DeleteSelected, () => SelectedProject is not null);

        Projects = new ObservableCollection<Project>(_dataService.GetProjects());
    }

    public event Action? DataChanged;

    public ObservableCollection<Project> Projects { get; }

    public Project? SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (!SetProperty(ref _selectedProject, value))
            {
                return;
            }

            DeleteCommand.NotifyCanExecuteChanged();
            if (value is null)
            {
                return;
            }

            ProjectNo = value.ProjectNo;
            Name = value.Name;
            Description = value.Description;
            StartDate = value.StartDate;
            EndDate = value.EndDate;
            LastAction = $"Loaded {value.ProjectNo} for edit";
        }
    }

    public string ProjectNo
    {
        get => _projectNo;
        set => SetProperty(ref _projectNo, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public string LastAction
    {
        get => _lastAction;
        private set => SetProperty(ref _lastAction, value);
    }

    public RelayCommand AddOrUpdateCommand { get; }

    public RelayCommand ClearFormCommand { get; }

    public RelayCommand DeleteCommand { get; }

    private void AddOrUpdate()
    {
        if (string.IsNullOrWhiteSpace(ProjectNo) || string.IsNullOrWhiteSpace(Name))
        {
            LastAction = "Project # and Name are required";
            return;
        }

        if (SelectedProject is null)
        {
            var item = _dataService.AddProject(new Project
            {
                ProjectNo = ProjectNo.Trim(),
                Name = Name.Trim(),
                Description = Description.Trim(),
                StartDate = StartDate,
                EndDate = EndDate
            });

            Projects.Add(item);
            LastAction = $"Added project {item.ProjectNo}";
            ClearForm();
            DataChanged?.Invoke();
            return;
        }

        SelectedProject.ProjectNo = ProjectNo.Trim();
        SelectedProject.Name = Name.Trim();
        SelectedProject.Description = Description.Trim();
        SelectedProject.StartDate = StartDate;
        SelectedProject.EndDate = EndDate;
        _dataService.UpdateProject(SelectedProject);

        // Force grid refresh for edited row.
        var index = Projects.IndexOf(SelectedProject);
        if (index >= 0)
        {
            Projects[index] = SelectedProject;
        }

        LastAction = $"Updated project {SelectedProject.ProjectNo}";
        ClearForm();
        DataChanged?.Invoke();
    }

    private void DeleteSelected()
    {
        if (SelectedProject is null)
        {
            LastAction = "No project selected";
            return;
        }

        var projectNo = SelectedProject.ProjectNo;
        _dataService.DeleteProject(SelectedProject.Id);
        Projects.Remove(SelectedProject);
        ClearForm();
        LastAction = $"Deleted project {projectNo}";
        DataChanged?.Invoke();
    }

    private void ClearForm()
    {
        SelectedProject = null;
        ProjectNo = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        StartDate = DateTime.Today;
        EndDate = null;
    }
}
