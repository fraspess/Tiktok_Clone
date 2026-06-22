import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'

export interface ILogin{
    login: string
    password: string
}
// Define a service using a base URL and expected endpoints
export const testApi = createApi({
    reducerPath: 'pokemonApi',
    baseQuery: fetchBaseQuery({
        baseUrl: 'http://localhost:5189/api/users',
        credentials: "include"
    }),
    endpoints: (builder) => ({
        login: builder.mutation<string, ILogin>({
            query: (body) => ({
                url: "login",
                method: "POST",
                body: body
            })
        }),
        refresh: builder.mutation<string, void>({
            query:() => ({
                url: "refresh",
                method: "POST",
            })
        })
    }),
})

// Export hooks for usage in functional components, which are
// auto-generated based on the defined endpoints
export const {
    useLoginMutation,
    useRefreshMutation
} = testApi