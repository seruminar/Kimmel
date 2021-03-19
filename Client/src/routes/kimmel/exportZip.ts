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
      const exportResponse = await wretch(process.env.KIMMEL_EXPORTZIP_ENDPOINT)
        .polyfills({
          fetch,
        })
        .post(JSON.parse(originalBody))
        .blob();

      response.writeHead(200, {
        "Content-Type": "application/zip",
      });

      (exportResponse.stream() as any).on("data", (chunk) => {
        response.write(chunk);
        response.end();
      });
    } catch (error) {
      switch (error.code) {
        case "ECONNREFUSED":
          response.statusCode = 404;
          break;

        default:
          response.statusCode = error.status;
          break;
      }

      response.end(JSON.stringify(error));
    }
  });
}
