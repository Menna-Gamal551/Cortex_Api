
using AutoMapper;
using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Data.Models.DTOs.DeviceDTOs;
using Cortex_Api.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Cortex_Api.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IDeviceSignatureVerifier _signatureVerifier;

        public DeviceService(AppDbContext db, IMapper mapper, IDeviceSignatureVerifier signatureVerifier)
        {
            _db = db;
            _mapper = mapper;
            _signatureVerifier = signatureVerifier;
        }

        
        public async Task<StartPairingResponseDto> StartPairingAsync(int userId,
            StartPairingRequestDto request)
        {
            
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

           
            if (user.DeviceId != null)
                throw new InvalidOperationException(
                    "This account already has a paired device.");

            
            var device = await _db.Devices
                .FirstOrDefaultAsync(d => d.DeviceUid == request.DeviceUid);

            if (device != null)
            {
                
                bool isAssigned = await _db.Users
                    .AnyAsync(u => u.DeviceId == device.Id);

                if (device.IsPaired || isAssigned)
                    throw new InvalidOperationException(
                        "This device is already paired.");

                
                if (device.PublicKey != request.PublicKey)
                    throw new InvalidOperationException(
                        "The device public key does not match the registered key.");
            }
            else
            {
               
                device = new Device
                {
                    DeviceUid = request.DeviceUid,
                    PublicKey = request.PublicKey,
                    FirmwareVersion = request.FirmwareVersion,
                    IsPaired = false
                };

                _db.Devices.Add(device);

                await _db.SaveChangesAsync();
            }

            
            var oldAttempts = await _db.DevicePairingAttempts
                .Where(p =>
                    p.UserId == userId &&
                    p.DeviceId == device.Id &&
                    !p.IsUsed)
                .ToListAsync();

            foreach (var oldAttempt in oldAttempts)
            {
                oldAttempt.IsUsed = true;
            }

            
            byte[] challengeBytes =
                RandomNumberGenerator.GetBytes(32);

            string challenge =
                Convert.ToBase64String(challengeBytes);

            
            string pairingId = Guid.NewGuid().ToString("N");

            DateTime now = DateTime.UtcNow;

            
            var pairingAttempt = new DevicePairingAttempt
            {
                PairingId = pairingId,
                Challenge = challenge,
                UserId = userId,
                DeviceId = device.Id,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(2),
                IsUsed = false
            };

            _db.DevicePairingAttempts.Add(pairingAttempt);

            await _db.SaveChangesAsync();

            return new StartPairingResponseDto
            {
                PairingId = pairingId,
                Challenge = challenge,
                ExpiresAt = pairingAttempt.ExpiresAt
            };
        }


       
        public async Task<bool> CompletePairingAsync(
            int userId,
            CompletePairingRequestDto request)
        {
            
            var attempt = await _db.DevicePairingAttempts
                .Include(p => p.Device)
                .FirstOrDefaultAsync(p =>
                    p.PairingId == request.PairingId &&
                    p.UserId == userId);

            if (attempt == null)
                throw new KeyNotFoundException(
                    "Pairing attempt not found.");

            
            if (attempt.IsUsed)
                throw new InvalidOperationException(
                    "This pairing attempt has already been used.");

            
            if (attempt.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidOperationException(
                    "Pairing challenge has expired.");

            
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var device = attempt.Device;

           
            if (user.DeviceId != null)
                throw new InvalidOperationException(
                    "This account already has a paired device.");

           
            bool isAssigned = await _db.Users
                .AnyAsync(u => u.DeviceId == device.Id);

            if (device.IsPaired || isAssigned)
                throw new InvalidOperationException(
                    "This device is already paired.");

            
            attempt.IsUsed = true;

            await _db.SaveChangesAsync();

            
            bool isValidSignature = _signatureVerifier.Verify(
                device.PublicKey,
                attempt.Challenge,
                request.Signature);

            if (!isValidSignature)
                return false;

           
            user.DeviceId = device.Id;

            device.IsPaired = true;
            device.PairedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return true;
        }


       
        public async Task<DeviceDto?> GetMyDeviceAsync(int userId)
        {
            
            var user = await _db.Users
                .Include(u => u.Device)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

           
            if (user.Device == null)
                return null;

           
            return _mapper.Map<DeviceDto>(user.Device);
        }
    }
}
