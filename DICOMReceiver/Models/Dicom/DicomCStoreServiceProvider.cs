using DICOMReceiver.Models.Entities;
using DICOMReceiver.Models;
using FellowOakDicom;
using FellowOakDicom.Network;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class DicomCStoreServiceProvider : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
{
    public DicomCStoreServiceProvider(INetworkStream stream, Encoding fallbackEncoding, ILogger logger, DicomServiceDependencies dependencies)
        : base(stream, fallbackEncoding, logger, dependencies) { }

    public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
    {
        foreach (var pc in association.PresentationContexts)
        {
            if (pc.AbstractSyntax == DicomUID.Verification)
            {
                pc.AcceptTransferSyntaxes(DicomTransferSyntax.ImplicitVRLittleEndian);
            }
            else
            {
                pc.AcceptTransferSyntaxes(
                    DicomTransferSyntax.ExplicitVRLittleEndian,
                    DicomTransferSyntax.ImplicitVRLittleEndian,
                    DicomTransferSyntax.ExplicitVRBigEndian);
            }
        }
        return SendAssociationAcceptAsync(association);
    }

    public Task OnReceiveAssociationReleaseRequestAsync()
    {
        return SendAssociationReleaseResponseAsync();
    }

    public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason) { }

    public void OnConnectionClosed(Exception exception) { }

    // ✅ Handle Echo requests
    public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
    {
        return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.Success));
    }

    // ✅ Handle Store requests
    public Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
    {
        try
        {
            var dbHandler = new DBHandler();

            var patient = new Patient
            {
                PatientID = request.Dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty),
                PatientName = request.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty),
                PatientSex = request.Dataset.GetSingleValueOrDefault(DicomTag.PatientSex, string.Empty),
                PatientBirthDate = request.Dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, string.Empty),
                PatientAge = request.Dataset.GetSingleValueOrDefault(DicomTag.PatientAge, string.Empty),
                IssuerOfPatientID = request.Dataset.GetSingleValueOrDefault(DicomTag.IssuerOfPatientID, string.Empty)
            };
            dbHandler.AddPatient(patient);

            var study = new Study
            {
                StudyInstanceUID = request.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                StudyID = request.Dataset.GetSingleValueOrDefault(DicomTag.StudyID, string.Empty),
                StudyDate = request.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty),
                StudyTime = request.Dataset.GetSingleValueOrDefault(DicomTag.StudyTime, string.Empty),
                StudyDescription = request.Dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, string.Empty),
                ReferringPhysicianName = request.Dataset.GetSingleValueOrDefault(DicomTag.ReferringPhysicianName, string.Empty),
                AccessionNumber = request.Dataset.GetSingleValueOrDefault(DicomTag.AccessionNumber, string.Empty),
                ModalitiesInStudy = request.Dataset.GetSingleValueOrDefault(DicomTag.ModalitiesInStudy, string.Empty),
                PatientId = patient.Id
            };
            dbHandler.AddStudy(study);

            var series = new Series
            {
                SeriesInstanceUID = request.Dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty),
                SeriesNumber = request.Dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, string.Empty),
                Modality = request.Dataset.GetSingleValueOrDefault(DicomTag.Modality, string.Empty),
                SeriesDescription = request.Dataset.GetSingleValueOrDefault(DicomTag.SeriesDescription, string.Empty),
                StudyId = study.Id
            };
            dbHandler.AddSeries(series);

            var fileHandler = new FileHandler();
            string folderPath = "ReceivedDicoms";
            string fileName = $"{study.StudyID}.{series.SeriesNumber}.{request.SOPInstanceUID.UID}.dcm";

            using var ms = new MemoryStream();
            request.File.Save(ms);
            fileHandler.SaveDicomFile(ms.ToArray(), folderPath, fileName);

            return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.Success));
        }
        catch
        {
            return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.ProcessingFailure));
        }
    }

    public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
    {
        return Task.CompletedTask;
    }
}
