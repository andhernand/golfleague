﻿using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Application.Services;

public interface IGolferService
{
    Task<GolferResponse> CreateAsync(CreateGolferRequest request, CancellationToken token = default);
    Task<GolferResponse?> GetGolferByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<GolferResponse>> GetAllGolfersAsync(CancellationToken token = default);
    Task<GolferResponse?> UpdateAsync(int id, UpdateGolferRequest request, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token = default);
}