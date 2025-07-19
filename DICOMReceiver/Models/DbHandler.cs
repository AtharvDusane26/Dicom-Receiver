using DBConfig;
using DICOMReceiver.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models
{
    public interface IDbHandler
    {
        void AddPatient(Patient patient);
        Patient GetPatientById(int id);
        List<Patient> GetAllPatients();
        void UpdatePatient(Patient patient);
        void DeletePatientById(int id);

        void AddStudy(Study study);
        Study GetStudyById(int id);
        List<Study> GetStudiesByPatientId(int patientId);
        void UpdateStudy(Study study);
        void DeleteStudyById(int id);

        void AddSeries(Series series);
        Series GetSeriesById(int id);
        List<Series> GetSeriesByStudyId(int studyId);
        void UpdateSeries(Series series);
        void DeleteSeriesById(int id);

        void AddNode(Nodes node);
        Nodes GetNodeById(int id);
        List<Nodes> GetAllNodes();
        void UpdateNode(Nodes node);
        void DeleteNodeById(int id);
    }
    public class DBHandler : IDbHandler
    {

        public void AddPatient(Patient patient)
        {
            DbEntity.Instance.Insert(patient);
        }

        public Patient GetPatientById(int id)
        {
            return DbEntity.Instance.GetSingle<Patient>(id);
        }

        public List<Patient> GetAllPatients()
        {
            return DbEntity.Instance.GetAll<Patient>().ToList();
        }

        public void UpdatePatient(Patient patient)
        {
            DbEntity.Instance.Insert(patient); 
        }

        public void DeletePatientById(int id)
        {
            var patient = DbEntity.Instance.GetSingle<Patient>(id);
            if (patient != null)
                DbEntity.Instance.Delete(patient);
        }

        public void AddStudy(Study study)
        {
            DbEntity.Instance.Insert(study);
        }

        public Study GetStudyById(int id)
        {
            return DbEntity.Instance.GetSingle<Study>(id);
        }

        public List<Study> GetStudiesByPatientId(int patientId)
        {
            return DbEntity.Instance.GetFiltered<Study>(x => x.PatientId == patientId).ToList();
        }

        public void UpdateStudy(Study study)
        {
            DbEntity.Instance.Insert(study);
        }

        public void DeleteStudyById(int id)
        {
            var study = DbEntity.Instance.GetSingle<Study>(id);
            if (study != null)
                DbEntity.Instance.Delete(study);
        }

        public void AddSeries(Series series)
        {
            DbEntity.Instance.Insert(series);
        }

        public Series GetSeriesById(int id)
        {
            return DbEntity.Instance.GetSingle<Series>(id);
        }

        public List<Series> GetSeriesByStudyId(int studyId)
        {
            return DbEntity.Instance.GetFiltered<Series>(x => x.StudyId == studyId).ToList();
        }

        public void UpdateSeries(Series series)
        {
            DbEntity.Instance.Insert(series);
        }

        public void DeleteSeriesById(int id)
        {
            var series = DbEntity.Instance.GetSingle<Series>(id);
            if (series != null)
                DbEntity.Instance.Delete(series);
        }

        public void AddNode(Nodes node)
        {
            DbEntity.Instance.Insert(node);
        }

        public Nodes GetNodeById(int id)
        {
            return DbEntity.Instance.GetSingle<Nodes>(id);
        }

        public List<Nodes> GetAllNodes()
        {
            return DbEntity.Instance.GetAll<Nodes>().ToList();
        }

        public void UpdateNode(Nodes node)
        {
            DbEntity.Instance.Insert(node); 
        }

        public void DeleteNodeById(int id)
        {
            var node = DbEntity.Instance.GetSingle<Nodes>(id);
            if (node != null)
                DbEntity.Instance.Delete(node);
        }
    }




}
