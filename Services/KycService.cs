using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CubeEnergy.Models;
using CubeEnergy.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public class KycService
    {
        private readonly IKycRepository _kycRepository;
        private readonly Cloudinary _cloudinary;

        public KycService(IKycRepository kycRepository, Cloudinary cloudinary)
        {
            _kycRepository = kycRepository;
            _cloudinary = cloudinary;
        }

        public async Task<string> CreateKycAsync(KycDto kycDto)
        {
            // Upload files to Cloudinary and get URLs
            var bankStatementUrl = await UploadFileAsync(kycDto.BankStatement);
            var passportDataPageUrl = await UploadFileAsync(kycDto.PassportDataPage);
            var nationalIDUrl = await UploadFileAsync(kycDto.NationalID);

            // Save KYC data to the database
            var kyc = new Kyc
            {
                Email = kycDto.Email,
                BankStatementUrl = bankStatementUrl,
                PassportDataPageUrl = passportDataPageUrl,
                NationalIDUrl = nationalIDUrl
            };

            await _kycRepository.CreateKycAsync(kyc);

            return $"KYC Data Uploaded Successfully. Bank Statement URL: {bankStatementUrl}, Passport Data Page URL: {passportDataPageUrl}, National ID URL: {nationalIDUrl}";
        }

        public async Task<string> UpdateKycAsync(int id, KycDto kycDto)
        {
            // Upload files to Cloudinary and get URLs
            var bankStatementUrl = await UploadFileAsync(kycDto.BankStatement);
            var passportDataPageUrl = await UploadFileAsync(kycDto.PassportDataPage);
            var nationalIDUrl = await UploadFileAsync(kycDto.NationalID);

            // Update KYC data in the database
            var kyc = new Kyc
            {
                Id = id,
                Email = kycDto.Email,
                BankStatementUrl = bankStatementUrl,
                PassportDataPageUrl = passportDataPageUrl,
                NationalIDUrl = nationalIDUrl
            };

            await _kycRepository.UpdateKycAsync(kyc);

            return $"KYC Data Updated Successfully. Bank Statement URL: {bankStatementUrl}, Passport Data Page URL: {passportDataPageUrl}, National ID URL: {nationalIDUrl}";
        }

        public async Task<bool> DeleteKycAsync(string email, int id)
        {
            return await _kycRepository.DeleteKycAsync(email, id);
        }

        public async Task<Kyc> GetKycByEmailAsync(string email)
        {
            return await _kycRepository.GetKycByEmailAsync(email);
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            throw new Exception("File upload to Cloudinary failed.");
        }
    }
}
