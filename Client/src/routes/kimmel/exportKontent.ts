import type { SapperRequest, SapperResponse } from "@sapper/server";
import fetch from 'node-fetch';
import wretch from 'wretch';

export function post(
  request: SapperRequest,
  response: SapperResponse,
  next: () => void
) {
  let originalBody = "";

  request.on("data", (chunk) => {
    originalBody += chunk;
  });

  request.on("end", async () => {
    try {
      const exportResponse = await wretch(
        process.env.KIMMEL_EXPORTKONTENT_ENDPOINT
      )
        .polyfills({
          fetch,
        })
        .post(JSON.parse(originalBody))
        .text();

      response.setHeader("Content-Type", "application/json");
      response.end(exportResponse);
    } catch (error) {
      switch (error.code) {
        case "ECONNREFUSED":
          response.statusCode = 404;
          break;

        default:
          response.statusCode = 400;
          break;
      }

      response.end(JSON.stringify(error));
    }
  });
}
