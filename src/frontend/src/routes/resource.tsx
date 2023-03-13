import { useFetcher } from "@remix-run/react";
import { useEffect } from "react";
import { type loader as resourceListLoader } from "~/routes/resource.list";
import { type action as resourceCreateAction } from "~/routes/resource.new";
import { type action as resourceDownloadAction } from "~/routes/resource.download";

export default function Resource() {
  const listFetcher = useFetcher<typeof resourceListLoader>();
  const createFetcher = useFetcher<typeof resourceCreateAction>();
  const downloadFetcher = useFetcher<typeof resourceDownloadAction>();

  useEffect(() => {
    if (listFetcher.type === "init") {
      listFetcher.load("/resource/list");
    }
  }, [listFetcher]);

  return (
    <div className="p-4">
      <div className="py-2 flex flex-col items-center">
        <createFetcher.Form
          action="/resource/new"
          method="post"
          className="w-fit flex flex-col"
        >
          <input
            type="text"
            name="url"
            placeholder="URL"
            className="border px-2"
          />
          <button type="submit" className="border">
            Submit
          </button>
        </createFetcher.Form>
        <p>{createFetcher.data?.id || "."}</p>
      </div>

      <div className="flex flex-col-reverse gap-2 max-w-7xl mx-auto">
        {listFetcher.data
          ? listFetcher.data.resources.map((r) => (
              <div key={r.id} className="border">
                <p className="font-mono">{r.id}</p>
                <p>{r.url}</p>
                <button type="submit" form={`download-${r.id}`}>
                  Download
                </button>
                <downloadFetcher.Form
                  hidden
                  id={`download-${r.id}`}
                  action="/resource/download"
                  method="post"
                >
                  <input type="text" name="id" value={r.id} />
                </downloadFetcher.Form>
              </div>
            ))
          : null}
      </div>
    </div>
  );
}
