import { useCallback, useEffect, useRef } from 'react'
import useFetch from '@/shared/lib/useFetch'
import useAppSelector from '@/shared/lib/useAppSelector'
import { notesApi, setMetaData, setNotes, appendNotes, reset, setIds } from '@/entities/note'
import store from '@/shared/lib/store'
import { useWindowEvent, useDebouncedCallback } from '@mantine/hooks'
import { useNavigationType, useSearchParams } from 'react-router-dom'
import rison from 'rison'
import updateFromUrl from '../../model/updateFromUrl'
import getUrlParams from '../../model/getUrlParams'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import NotesMasonry from '../notesMasonry'
import Loading from '@/shared/ui/loading'
import { Center, Button } from '@mantine/core'

import type { paths } from '@/shared/api'
import useCustomEventListener from '@/shared/lib/useCustomEventListener'
import { events } from '@/shared/config'

function NotesList() {
  const firstUpdate = useRef(true)
  const navType = useNavigationType()
  const [searchParams, setSearchParams] = useSearchParams()
  const list = searchParams.get('list')
  const { request, isLoading } = useFetch(notesApi.searchNotes, { concurrent: true })
  const hasNotes = useAppSelector((state) => state.notes.ids.length > 0)
  const canLoadMore = useAppSelector((state) => state.notes.metadata.canLoadMore)
  const color = useCurrentColor()

  const searchNotes = useCallback(
    (page = 1) => {
      const allNotesSectionName = store.getState().sections.allNotesSection.name
      const section = store.getState().sections.current?.name
      const filters = store.getState().notes.filters
      const searchParams: paths['/api/notes']['get']['parameters']['query'] = {
        Page: page,
      }

      if (filters.book) {
        searchParams.Book = filters.book
      }

      if (filters.tags.length > 0) {
        searchParams.Tags = [...filters.tags]
      }

      if (filters.fromDate) {
        searchParams.FromDate = filters.fromDate
      }

      if (filters.toDate) {
        searchParams.ToDate = filters.toDate
      }

      if (filters.searchTerm.length > 2) {
        searchParams.SearchTerm = filters.searchTerm
      }

      if (filters.inRandomOrder) {
        searchParams.InRandomOrder = filters.inRandomOrder
      }

      if (filters.withoutBook) {
        searchParams.WithoutBook = filters.withoutBook
      }

      if (filters.withoutTags) {
        searchParams.WithoutTags = filters.withoutTags
      }

      searchParams.Page = page
      if (section && section !== allNotesSectionName) {
        searchParams.Section = section
      }

      if (page === 1 && store.getState().notes.ids.length > 0) {
        store.dispatch(setIds([]))
      }

      request(searchParams, ({ data }) => {
        if (data) {
          const notes = data.data ?? []

          if (page === 1) {
            store.dispatch(setNotes(notes))
          } else {
            store.dispatch(appendNotes(notes))
          }

          store.dispatch(
            setMetaData({
              total: data.total ?? 0,
              canLoadMore: data.canLoadMore ?? false,
              page: data.page ?? 1,
              keywords: data.keywords ?? [],
              foundWholePhrase: data.foundWholePhrase ?? false,
            }),
          )
        }
      })
    },
    [request],
  )

  const handleLoadMore = useCallback(() => {
    if (store.getState().notes.metadata.canLoadMore) {
      searchNotes(store.getState().notes.metadata.page + 1)
    }
  }, [searchNotes])

  const handleLoadMoreDebounced = useDebouncedCallback(() => {
    handleLoadMore()
  }, 500)

  const updateUrl = useCallback(() => {
    const listParams = getUrlParams()

    setSearchParams((searchParams) => {
      searchParams.set('list', rison.encode(listParams))
      return searchParams
    })
  }, [setSearchParams])

  useEffect(() => {
    return () => {
      store.dispatch(reset())
    }
  }, [])

  useEffect(() => {
    if (firstUpdate.current) {
      firstUpdate.current = false
      if (list) {
        updateFromUrl(list)
        searchNotes()
      } else {
        updateUrl()
        searchNotes()
      }
    } else {
      if (navType === 'POP' && list) {
        updateFromUrl(list)
        searchNotes()
      }
    }
  }, [list, navType, setSearchParams, searchNotes, updateUrl])

  useCustomEventListener(events.notesList.search, () => {
    updateUrl()
    searchNotes()
  })

  useWindowEvent('scroll', () => {
    if (window.scrollY + window.innerHeight >= document.body.scrollHeight - 100) {
      handleLoadMoreDebounced()
    }
  })

  return (
    <>
      {hasNotes && <NotesMasonry />}
      {!hasNotes && isLoading && <Loading full />}
      {hasNotes && canLoadMore && (
        <Center>
          <Button
            color={color}
            mt={20}
            onClick={handleLoadMore}
            loading={isLoading}
          >
            Load more
          </Button>
        </Center>
      )}
    </>
  )
}

export default NotesList
