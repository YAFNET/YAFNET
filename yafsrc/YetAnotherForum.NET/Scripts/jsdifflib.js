/***
This is part of jsdifflib v1.0. <http://snowtide.com/jsdifflib>

Copyright (c) 2007, Snowtide Informatics Systems, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

	* Redistributions of source code must retain the above copyright notice, this
		list of conditions and the following disclaimer.
	* Redistributions in binary form must reproduce the above copyright notice,
		this list of conditions and the following disclaimer in the documentation
		and/or other materials provided with the distribution.
	* Neither the name of the Snowtide Informatics Systems nor the names of its
		contributors may be used to endorse or promote products derived from this
		software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
***/
/* Author: Chas Emerick <cemerick@snowtide.com> */
__whitespace = { " ": true, "\t": true, "\n": true, "\f": true, "\r": true };

difflib = {
    defaultJunkFunction: function (c) {
        return c in __whitespace;
    },

    stripLinebreaks: function (str) { return str.replace(/^[\n\r]*|[\n\r]*$/g, ""); },

    stringAsLines: function (str) {
        var lfpos = str.indexOf("\n");
        var crpos = str.indexOf("\r");
        var linebreak = ((lfpos > -1 && crpos > -1) || crpos < 0) ? "\n" : "\r";

        var lines = str.split(linebreak);
        for (var i = 0; i < lines.length; i++) {
            lines[i] = difflib.stripLinebreaks(lines[i]);
        }

        return lines;
    },

    // iteration-based reduce implementation
    __reduce: function (func, list, initial) {
        if (initial != null) {
            var value = initial;
            var idx = 0;
        } else if (list) {
            var value = list[0];
            var idx = 1;
        } else {
            return null;
        }

        for (; idx < list.length; idx++) {
            value = func(value, list[idx]);
        }

        return value;
    },

    // comparison function for sorting lists of numeric tuples
    __ntuplecomp: function (a, b) {
        var mlen = Math.max(a.length, b.length);
        for (var i = 0; i < mlen; i++) {
            if (a[i] < b[i]) return -1;
            if (a[i] > b[i]) return 1;
        }

        return a.length == b.length ? 0 : (a.length < b.length ? -1 : 1);
    },

    __calculate_ratio: function (matches, length) {
        return length ? 2.0 * matches / length : 1.0;
    },

    // returns a function that returns true if a key passed to the returned function
    // is in the dict (js object) provided to this function; replaces being able to
    // carry around dict.has_key in python...
    __isindict: function (dict) {
        return function (key) { return key in dict; };
    },

    // replacement for python's dict.get function -- need easy default values
    __dictget: function (dict, key, defaultValue) {
        return key in dict ? dict[key] : defaultValue;
    },

    SequenceMatcher: function (a, b, isjunk) {
        this.set_seqs = function (a, b) {
            this.set_seq1(a);
            this.set_seq2(b);
        }

        this.set_seq1 = function (a) {
            if (a == this.a) return;
            this.a = a;
            this.matching_blocks = this.opcodes = null;
        }

        this.set_seq2 = function (b) {
            if (b == this.b) return;
            this.b = b;
            this.matching_blocks = this.opcodes = this.fullbcount = null;
            this.__chain_b();
        }

        this.__chain_b = function () {
            var b = this.b;
            var n = b.length;
            var b2j = this.b2j = {};
            var populardict = {};
            for (var i = 0; i < b.length; i++) {
                var elt = b[i];
                if (elt in b2j) {
                    var indices = b2j[elt];
                    if (n >= 200 && indices.length * 100 > n) {
                        populardict[elt] = 1;
                        delete b2j[elt];
                    } else {
                        indices.push(i);
                    }
                } else {
                    b2j[elt] = [i];
                }
            }

            for (var elt in populardict)
                delete b2j[elt];

            var isjunk = this.isjunk;
            var junkdict = {};
            if (isjunk) {
                for (var elt in populardict) {
                    if (isjunk(elt)) {
                        junkdict[elt] = 1;
                        delete populardict[elt];
                    }
                }
                for (var elt in b2j) {
                    if (isjunk(elt)) {
                        junkdict[elt] = 1;
                        delete b2j[elt];
                    }
                }
            }

            this.isbjunk = difflib.__isindict(junkdict);
            this.isbpopular = difflib.__isindict(populardict);
        }

        this.find_longest_match = function (alo, ahi, blo, bhi) {
            var a = this.a;
            var b = this.b;
            var b2j = this.b2j;
            var isbjunk = this.isbjunk;
            var besti = alo;
            var bestj = blo;
            var bestsize = 0;
            var j = null;

            var j2len = {};
            var nothing = [];
            for (var i = alo; i < ahi; i++) {
                var newj2len = {};
                var jdict = difflib.__dictget(b2j, a[i], nothing);
                for (var jkey in jdict) {
                    j = jdict[jkey];
                    if (j < blo) continue;
                    if (j >= bhi) break;
                    newj2len[j] = k = difflib.__dictget(j2len, j - 1, 0) + 1;
                    if (k > bestsize) {
                        besti = i - k + 1;
                        bestj = j - k + 1;
                        bestsize = k;
                    }
                }
                j2len = newj2len;
            }

            while (besti > alo && bestj > blo && !isbjunk(b[bestj - 1]) && a[besti - 1] == b[bestj - 1]) {
                besti--;
                bestj--;
                bestsize++;
            }

            while (besti + bestsize < ahi && bestj + bestsize < bhi &&
					!isbjunk(b[bestj + bestsize]) &&
					a[besti + bestsize] == b[bestj + bestsize]) {
                bestsize++;
            }

            while (besti > alo && bestj > blo && isbjunk(b[bestj - 1]) && a[besti - 1] == b[bestj - 1]) {
                besti--;
                bestj--;
                bestsize++;
            }

            while (besti + bestsize < ahi && bestj + bestsize < bhi && isbjunk(b[bestj + bestsize]) &&
				  a[besti + bestsize] == b[bestj + bestsize]) {
                bestsize++;
            }

            return [besti, bestj, bestsize];
        }

        this.get_matching_blocks = function () {
            if (this.matching_blocks != null) return this.matching_blocks;
            var la = this.a.length;
            var lb = this.b.length;

            var queue = [[0, la, 0, lb]];
            var matching_blocks = [];
            var alo, ahi, blo, bhi, qi, i, j, k, x;
            while (queue.length) {
                qi = queue.pop();
                alo = qi[0];
                ahi = qi[1];
                blo = qi[2];
                bhi = qi[3];
                x = this.find_longest_match(alo, ahi, blo, bhi);
                i = x[0];
                j = x[1];
                k = x[2];

                if (k) {
                    matching_blocks.push(x);
                    if (alo < i && blo < j)
                        queue.push([alo, i, blo, j]);
                    if (i + k < ahi && j + k < bhi)
                        queue.push([i + k, ahi, j + k, bhi]);
                }
            }

            matching_blocks.sort(difflib.__ntuplecomp);

            var i1 = j1 = k1 = block = 0;
            var non_adjacent = [];
            for (var idx in matching_blocks) {
                block = matching_blocks[idx];
                i2 = block[0];
                j2 = block[1];
                k2 = block[2];
                if (i1 + k1 == i2 && j1 + k1 == j2) {
                    k1 += k2;
                } else {
                    if (k1) non_adjacent.push([i1, j1, k1]);
                    i1 = i2;
                    j1 = j2;
                    k1 = k2;
                }
            }

            if (k1) non_adjacent.push([i1, j1, k1]);

            non_adjacent.push([la, lb, 0]);
            this.matching_blocks = non_adjacent;
            return this.matching_blocks;
        }

        this.get_opcodes = function () {
            if (this.opcodes != null) return this.opcodes;
            var i = 0;
            var j = 0;
            var answer = [];
            this.opcodes = answer;
            var block, ai, bj, size, tag;
            var blocks = this.get_matching_blocks();
            for (var idx in blocks) {
                block = blocks[idx];
                ai = block[0];
                bj = block[1];
                size = block[2];
                tag = '';
                if (i < ai && j < bj) {
                    tag = 'replace';
                } else if (i < ai) {
                    tag = 'delete';
                } else if (j < bj) {
                    tag = 'insert';
                }
                if (tag) answer.push([tag, i, ai, j, bj]);
                i = ai + size;
                j = bj + size;

                if (size) answer.push(['equal', ai, i, bj, j]);
            }

            return answer;
        }

        // this is a generator function in the python lib, which of course is not supported in javascript
        // the reimplementation builds up the grouped opcodes into a list in their entirety and returns that.
        this.get_grouped_opcodes = function (n) {
            if (!n) n = 3;
            var codes = this.get_opcodes();
            if (!codes) codes = [["equal", 0, 1, 0, 1]];
            var code, tag, i1, i2, j1, j2;
            if (codes[0][0] == 'equal') {
                code = codes[0];
                tag = code[0];
                i1 = code[1];
                i2 = code[2];
                j1 = code[3];
                j2 = code[4];
                codes[0] = [tag, Math.max(i1, i2 - n), i2, Math.max(j1, j2 - n), j2];
            }
            if (codes[codes.length - 1][0] == 'equal') {
                code = codes[codes.length - 1];
                tag = code[0];
                i1 = code[1];
                i2 = code[2];
                j1 = code[3];
                j2 = code[4];
                codes[codes.length - 1] = [tag, i1, Math.min(i2, i1 + n), j1, Math.min(j2, j1 + n)];
            }

            var nn = n + n;
            var groups = [];
            for (var idx in codes) {
                code = codes[idx];
                tag = code[0];
                i1 = code[1];
                i2 = code[2];
                j1 = code[3];
                j2 = code[4];
                if (tag == 'equal' && i2 - i1 > nn) {
                    groups.push([tag, i1, Math.min(i2, i1 + n), j1, Math.min(j2, j1 + n)]);
                    i1 = Math.max(i1, i2 - n);
                    j1 = Math.max(j1, j2 - n);
                }

                groups.push([tag, i1, i2, j1, j2]);
            }

            if (groups && groups[groups.length - 1][0] == 'equal') groups.pop();

            return groups;
        }

        this.ratio = function () {
            matches = difflib.__reduce(
							function (sum, triple) { return sum + triple[triple.length - 1]; },
							this.get_matching_blocks(), 0);
            return difflib.__calculate_ratio(matches, this.a.length + this.b.length);
        }

        this.quick_ratio = function () {
            var fullbcount, elt;
            if (this.fullbcount == null) {
                this.fullbcount = fullbcount = {};
                for (var i = 0; i < this.b.length; i++) {
                    elt = this.b[i];
                    fullbcount[elt] = difflib.__dictget(fullbcount, elt, 0) + 1;
                }
            }
            fullbcount = this.fullbcount;

            var avail = {};
            var availhas = difflib.__isindict(avail);
            var matches = numb = 0;
            for (var i = 0; i < this.a.length; i++) {
                elt = this.a[i];
                if (availhas(elt)) {
                    numb = avail[elt];
                } else {
                    numb = difflib.__dictget(fullbcount, elt, 0);
                }
                avail[elt] = numb - 1;
                if (numb > 0) matches++;
            }

            return difflib.__calculate_ratio(matches, this.a.length + this.b.length);
        }

        this.real_quick_ratio = function () {
            var la = this.a.length;
            var lb = this.b.length;
            return _calculate_ratio(Math.min(la, lb), la + lb);
        }

        this.isjunk = isjunk ? isjunk : difflib.defaultJunkFunction;
        this.a = this.b = null;
        this.set_seqs(a, b);
    }
}

/***
This is part of jsdifflib v1.0. <http://snowtide.com/jsdifflib>

Copyright (c) 2007, Snowtide Informatics Systems, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

	* Redistributions of source code must retain the above copyright notice, this
		list of conditions and the following disclaimer.
	* Redistributions in binary form must reproduce the above copyright notice,
		this list of conditions and the following disclaimer in the documentation
		and/or other materials provided with the distribution.
	* Neither the name of the Snowtide Informatics Systems nor the names of its
		contributors may be used to endorse or promote products derived from this
		software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
***/
/* Author: Chas Emerick <cemerick@snowtide.com> */
/* Modified by Richard Bondi http://richardbondi.net */
diffview = {
    /**
	 * Builds and returns a visual diff view.  The single parameter, `params', should contain
	 * the following values:
	 *
	 * - baseTextLines: the array of strings that was used as the base text input to SequenceMatcher
	 * - newTextLines: the array of strings that was used as the new text input to SequenceMatcher
	 * - opcodes: the array of arrays returned by SequenceMatcher.get_opcodes()
	 * - baseTextName: the title to be displayed above the base text listing in the diff view; defaults
	 *	   to "Base Text"
	 * - newTextName: the title to be displayed above the new text listing in the diff view; defaults
	 *	   to "New Text"
	 * - contextSize: the number of lines of context to show around differences; by default, all lines
	 *	   are shown
	 * - viewType: if 0, a side-by-side diff view is generated (default); if 1, an inline diff view is
	 *	   generated
	 */
    buildView: function (params) {
        var baseTextLines = params.baseTextLines;
        var newTextLines = params.newTextLines;
        var opcodes = params.opcodes;
        var baseTextName = params.baseTextName ? params.baseTextName : "Base Text";
        var newTextName = params.newTextName ? params.newTextName : "New Text";
        var contextSize = params.contextSize;
        var inline = (params.viewType == 0 || params.viewType == 1) ? params.viewType : 0;

        if (baseTextLines == null)
            throw "Cannot build diff view; baseTextLines is not defined.";
        if (newTextLines == null)
            throw "Cannot build diff view; newTextLines is not defined.";
        if (!opcodes)
            throw "Canno build diff view; opcodes is not defined.";

        function celt(name, clazz) {
            var e = document.createElement(name);
            e.className = clazz;
            return e;
        }

        function telt(name, text) {
            var e = document.createElement(name);
            e.appendChild(document.createTextNode(text));
            return e;
        }

        function ctelt(name, clazz, text) {
            var e = document.createElement(name);
            e.className = clazz;
            e.innerHTML = text;
            return e;
        }

        var tdata = document.createElement("thead");
        var node = document.createElement("tr");
        tdata.appendChild(node);
        if (inline) {
            node.appendChild(document.createElement("th"));
            node.appendChild(document.createElement("th"));
            node.appendChild(ctelt("th", "texttitle", baseTextName + " vs. " + newTextName));
        } else {
            node.appendChild(document.createElement("th"));
            node.appendChild(ctelt("th", "texttitle", baseTextName));
            node.appendChild(document.createElement("th"));
            node.appendChild(ctelt("th", "texttitle", newTextName));
        }
        tdata = [tdata];

        var rows = [];
        var node2;

        /**
		 * Adds two cells to the given row; if the given row corresponds to a real
		 * line number (based on the line index tidx and the endpoint of the 
		 * range in question tend), then the cells will contain the line number
		 * and the line of text from textLines at position tidx (with the class of
		 * the second cell set to the name of the change represented), and tidx + 1 will
		 * be returned.	 Otherwise, tidx is returned, and two empty cells are added
		 * to the given row.
		 */
        function addCells(row, tidx, tend, textLines, change) {
            if (tidx < tend) {
                row.appendChild(telt("th", (tidx + 1).toString()));
                row.appendChild(ctelt("td", change, textLines[tidx].replace(/\t/g, "\u00a0\u00a0\u00a0\u00a0")));
                return tidx + 1;
            } else {
                row.appendChild(document.createElement("th"));
                row.appendChild(celt("td", "empty"));
                return tidx;
            }
        }

        function addCellsInline(row, tidx, tidx2, textLines, change) {
            row.appendChild(telt("th", tidx == null ? "" : (tidx + 1).toString()));
            row.appendChild(telt("th", tidx2 == null ? "" : (tidx2 + 1).toString()));
            row.appendChild(ctelt("td", change, textLines[tidx != null ? tidx : tidx2].replace(/\t/g, "\u00a0\u00a0\u00a0\u00a0")));
        }

        for (var idx = 0; idx < opcodes.length; idx++) {
            code = opcodes[idx];
            change = code[0];
            var b = code[1];
            var be = code[2];
            var n = code[3];
            var ne = code[4];
            var rowcnt = Math.max(be - b, ne - n);
            var toprows = [];
            var botrows = [];
            for (var i = 0; i < rowcnt; i++) {
                // jump ahead if we've alredy provided leading context or if this is the first range
                if (contextSize && opcodes.length > 1 && ((idx > 0 && i == contextSize) || (idx == 0 && i == 0)) && change == "equal") {
                    var jump = rowcnt - ((idx == 0 ? 1 : 2) * contextSize);
                    if (jump > 1) {
                        toprows.push(node = document.createElement("tr"));

                        b += jump;
                        n += jump;
                        i += jump - 1;
                        node.appendChild(telt("th", "..."));
                        if (!inline) node.appendChild(ctelt("td", "skip", ""));
                        node.appendChild(telt("th", "..."));
                        node.appendChild(ctelt("td", "skip", ""));

                        // skip last lines if they're all equal
                        if (idx + 1 == opcodes.length) {
                            break;
                        } else {
                            continue;
                        }
                    }
                }

                toprows.push(node = document.createElement("tr"));
                if (inline) {
                    if (change == "insert") {
                        addCellsInline(node, null, n++, newTextLines, change);
                    } else if (change == "replace") {
                        botrows.push(node2 = document.createElement("tr"));
                        if (b < be) addCellsInline(node, b++, null, baseTextLines, "delete");
                        if (n < ne) addCellsInline(node2, null, n++, newTextLines, "insert");
                    } else if (change == "delete") {
                        addCellsInline(node, b++, null, baseTextLines, change);
                    } else {
                        // equal
                        addCellsInline(node, b++, n++, baseTextLines, change);
                    }
                } else {
                    var wdiff = diffString2(b < be ? baseTextLines[b] : "", n < ne ? newTextLines[n] : "");
                    if (b < be) baseTextLines[b] = wdiff.o;
                    if (n < ne) newTextLines[n] = wdiff.n;
                    b = addCells(node, b, be, baseTextLines, change == "replace" ? "delete" : change);
                    n = addCells(node, n, ne, newTextLines, change == "replace" ? "insert" : change);
                }
            }

            for (var i = 0; i < toprows.length; i++) rows.push(toprows[i]);
            for (var i = 0; i < botrows.length; i++) rows.push(botrows[i]);
        }

        var msg = "combined <a href='http://snowtide.com/jsdifflib'>jsdifflib</a> ";
        msg += "and John Resig's <a href='http://ejohn.org/projects/javascript-diff-algorithm/'>diff</a> ";
        msg += "by <a href='http://richardbondi.net'>Richard Bondi</a>";
        rows.push(node = ctelt("th", "author", msg));
        node.setAttribute("colspan", inline ? 3 : 4);

        tdata.push(node = document.createElement("tbody"));
        for (var idx in rows) node.appendChild(rows[idx]);

        node = celt("table", "diff" + (inline ? " inlinediff" : ""));
        for (var idx in tdata) node.appendChild(tdata[idx]);
        return node;
    }
}

/*
 * Javascript Diff Algorithm
 *  By John Resig (http://ejohn.org/)
 *  Modified by Chu Alan "sprite"
 *  diffstring2 random colors removed by Richard Bondi for use with jsdifflib
 *
 * Released under the MIT license.
 *
 * More Info:
 *  http://ejohn.org/projects/javascript-diff-algorithm/
 *
 */

function escape(s) {
    var n = s;
    n = n.replace(/&/g, "&amp;");
    n = n.replace(/</g, "&lt;");
    n = n.replace(/>/g, "&gt;");
    n = n.replace(/"/g, "&quot;");

    return n;
}

function diffString(o, n) {
    o = o.replace(/\s+$/, '');
    n = n.replace(/\s+$/, '');

    var out = diff(o == "" ? [] : o.split(/\s+/), n == "" ? [] : n.split(/\s+/));
    var str = "";

    var oSpace = o.match(/\s+/g);
    if (oSpace == null) {
        oSpace = ["\n"];
    } else {
        oSpace.push("\n");
    }
    var nSpace = n.match(/\s+/g);
    if (nSpace == null) {
        nSpace = ["\n"];
    } else {
        nSpace.push("\n");
    }

    if (out.n.length == 0) {
        for (var i = 0; i < out.o.length; i++) {
            str += '<del>' + escape(out.o[i]) + oSpace[i] + "</del>";
        }
    } else {
        if (out.n[0].text == null) {
            for (n = 0; n < out.o.length && out.o[n].text == null; n++) {
                str += '<del>' + escape(out.o[n]) + oSpace[n] + "</del>";
            }
        }

        for (var i = 0; i < out.n.length; i++) {
            if (out.n[i].text == null) {
                str += '<ins>' + escape(out.n[i]) + nSpace[i] + "</ins>";
            } else {
                var pre = "";

                for (n = out.n[i].row + 1; n < out.o.length && out.o[n].text == null; n++) {
                    pre += '<del>' + escape(out.o[n]) + oSpace[n] + "</del>";
                }
                str += " " + out.n[i].text + nSpace[i] + pre;
            }
        }
    }

    return str;
}

function randomColor() {
    return "rgb(" + (Math.random() * 100) + "%, " +
                    (Math.random() * 100) + "%, " +
                    (Math.random() * 100) + "%)";
}
function diffString2(o, n) {
    o = o.replace(/\s+$/, '');
    n = n.replace(/\s+$/, '');

    var out = diff(o == "" ? [] : o.split(/\s+/), n == "" ? [] : n.split(/\s+/));

    var oSpace = o.match(/\s+/g);
    if (oSpace == null) {
        oSpace = ["\n"];
    } else {
        oSpace.push("\n");
    }
    var nSpace = n.match(/\s+/g);
    if (nSpace == null) {
        nSpace = ["\n"];
    } else {
        nSpace.push("\n");
    }

    var os = "";
    var colors = new Array();
    for (var i = 0; i < out.o.length; i++) {
        colors[i] = randomColor();

        if (out.o[i].text != null) {
            os += escape(out.o[i].text) + oSpace[i];
        } else {
            os += "<del>" + escape(out.o[i]) + oSpace[i] + "</del>";
        }
    }

    var ns = "";
    for (var i = 0; i < out.n.length; i++) {
        if (out.n[i].text != null) {
            ns += escape(out.n[i].text) + nSpace[i];
        } else {
            ns += "<ins>" + escape(out.n[i]) + nSpace[i] + "</ins>";
        }
    }

    return { o: os, n: ns };
}

function diff(o, n) {
    var ns = new Object();
    var os = new Object();

    for (var i = 0; i < n.length; i++) {
        if (ns[n[i]] == null)
            ns[n[i]] = { rows: new Array(), o: null };
        ns[n[i]].rows.push(i);
    }

    for (var i = 0; i < o.length; i++) {
        if (os[o[i]] == null)
            os[o[i]] = { rows: new Array(), n: null };
        os[o[i]].rows.push(i);
    }

    for (var i in ns) {
        if (ns[i].rows.length == 1 && typeof (os[i]) != "undefined" && os[i].rows.length == 1) {
            n[ns[i].rows[0]] = { text: n[ns[i].rows[0]], row: os[i].rows[0] };
            o[os[i].rows[0]] = { text: o[os[i].rows[0]], row: ns[i].rows[0] };
        }
    }

    for (var i = 0; i < n.length - 1; i++) {
        if (n[i].text != null && n[i + 1].text == null && n[i].row + 1 < o.length && o[n[i].row + 1].text == null &&
             n[i + 1] == o[n[i].row + 1]) {
            n[i + 1] = { text: n[i + 1], row: n[i].row + 1 };
            o[n[i].row + 1] = { text: o[n[i].row + 1], row: i + 1 };
        }
    }

    for (var i = n.length - 1; i > 0; i--) {
        if (n[i].text != null && n[i - 1].text == null && n[i].row > 0 && o[n[i].row - 1].text == null &&
             n[i - 1] == o[n[i].row - 1]) {
            n[i - 1] = { text: n[i - 1], row: n[i].row - 1 };
            o[n[i].row - 1] = { text: o[n[i].row - 1], row: i - 1 };
        }
    }

    return { o: o, n: n };
}